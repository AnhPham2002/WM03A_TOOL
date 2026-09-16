using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace WM03A
{
    /// <summary>
    /// Quản lý cổng COM cho bus RS485 dùng chung (nhiều loại frame khác nhau
    /// cùng xuất hiện trên bus: frame Modbus RTU của đồng hồ nước, frame
    /// telemetry riêng "5A..." của MCU, v.v.)
    ///
    /// Nguyên tắc tách frame: mỗi khoảng lặng RX_TIMEOUT_MS được coi là ranh
    /// giới kết thúc 1 frame. Khác với bản cũ, mỗi frame tách được sẽ được
    /// đẩy vào một HÀNG ĐỢI (_frameQueue) thay vì ghi đè lên một buffer dùng
    /// chung duy nhất. Nhờ vậy:
    ///   - Không còn tình trạng buffer bị Clear() giữa chừng khi 1 frame khác
    ///     đang được nhận dở (nguyên nhân gây mất dữ liệu, ví dụ frame "5A..."
    ///     bị cắt cụt chỉ còn lại "FF").
    ///   - Communicate() có thể lần lượt thử từng frame đã tách xong trong
    ///     hàng đợi (bỏ qua các frame không phải Modbus, ví dụ frame "5A...")
    ///     mà không làm mất frame Modbus đến sau/trước nó.
    /// </summary>
    public class SerialPortManager
    {
        private const int RX_TIMEOUT_MS = 100;

        // Số frame tối đa giữ trong hàng đợi khi chưa có ai lấy ra, tránh
        // phình bộ nhớ vô hạn nếu bus có nhiều frame "rác" không ai xử lý.
        private const int MAX_QUEUED_FRAMES = 50;

        private SerialPort _serialPort;

        // Bộ đệm gom byte cho FRAME ĐANG NHẬN DỞ (chưa hết khoảng lặng).
        private readonly List<byte> _rxAccumulator = new List<byte>();

        // Hàng đợi các frame ĐÃ HOÀN CHỈNH (đã đủ khoảng lặng RX_TIMEOUT_MS).
        private readonly Queue<byte[]> _frameQueue = new Queue<byte[]>();

        private readonly object _rxLock = new object();
        private readonly Timer _rxTimer;

        // Báo hiệu bất đồng bộ mỗi khi có 1 frame mới được đẩy vào hàng đợi.
        // Communicate/CommunicateAsync chờ trên semaphore này thay vì
        // Thread.Sleep polling → không còn block thread gọi (UI thread).
        private readonly SemaphoreSlim _frameSignal = new SemaphoreSlim(0);

        public event Action<byte[]> DataSent;
        public event Action<byte[]> DataReceived;

        public bool IsOpen
        {
            get
            {
                return _serialPort != null && _serialPort.IsOpen;
            }
        }

        public SerialPortManager()
        {
            _rxTimer = new Timer(
                RxTimerCallback,
                null,
                Timeout.Infinite,
                Timeout.Infinite);
        }

        public void Open(string portName)
        {
            if (IsOpen)
            {
                return;
            }

            _serialPort = new SerialPort(
                portName,
                19200,
                Parity.None,
                8,
                StopBits.One);

            _serialPort.DataReceived += SerialPort_DataReceived;

            _serialPort.Open();
        }

        public void Close()
        {
            if (_serialPort == null)
            {
                return;
            }

            _serialPort.DataReceived -= SerialPort_DataReceived;

            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            _serialPort.Dispose();
            _serialPort = null;

            _rxTimer.Change(
                Timeout.Infinite,
                Timeout.Infinite);

            lock (_rxLock)
            {
                _rxAccumulator.Clear();
                _frameQueue.Clear();
            }

            DrainSignal();
        }

        /// <summary>
        /// Đưa _frameSignal về đúng 0 (không còn "tín hiệu" tồn đọng ứng với
        /// các frame đã bị Clear() khỏi hàng đợi). Không block vì dùng
        /// Wait(0) — chỉ lấy những gì đã sẵn có ngay lập tức.
        /// </summary>
        private void DrainSignal()
        {
            while (_frameSignal.Wait(0))
            {
                // tiêu thụ hết các tín hiệu còn dư
            }
        }

        public void Send(byte[] data)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException(
                    "COM chưa được mở.");
            }

            if (data == null || data.Length == 0)
            {
                return;
            }

            _serialPort.Write(
                data,
                0,
                data.Length);

            DataSent?.Invoke(data);
        }

        /// <summary>
        /// Lấy ra 1 frame đã hoàn chỉnh (nếu có) trong hàng đợi, theo thứ tự
        /// FIFO. Trả về false nếu chưa có frame nào sẵn sàng.
        /// </summary>
        public bool Receive(out byte[] data)
        {
            lock (_rxLock)
            {
                if (_frameQueue.Count == 0)
                {
                    data = null;
                    return false;
                }

                data = _frameQueue.Dequeue();
                return true;
            }
        }

        /// <summary>
        /// Gửi txFrame và chờ 1 frame phản hồi hợp lệ trong tối đa timeoutMs,
        /// KHÔNG BLOCK thread gọi (dùng await thay vì Thread.Sleep).
        ///
        /// Vì bus có thể lẫn nhiều loại frame khác (ví dụ frame telemetry
        /// "5A..." của MCU), hàm sẽ thử LẦN LƯỢT từng frame tách được cho
        /// tới khi tìm được frame thỏa isValidFrame, hoặc hết thời gian chờ.
        /// Các frame không hợp lệ sẽ bị bỏ qua chứ không làm mất frame hợp
        /// lệ đến sau đó.
        ///
        /// Lưu ý: async method trong C# không được dùng tham số out/ref,
        /// nên kết quả trả về dạng tuple (Success, RxFrame) thay vì out.
        /// </summary>
        /// <param name="isValidFrame">
        /// Hàm kiểm tra 1 frame có phải là phản hồi mong đợi hay không.
        /// Nếu không truyền, mặc định dùng Protocol.Unpack như bản gốc.
        /// </param>
        public async Task<(bool Success, byte[] RxFrame)> CommunicateAsync(
            byte[] txFrame,
            int timeoutMs,
            Func<byte[], bool> isValidFrame = null,
            CancellationToken cancellationToken = default)
        {
            if (isValidFrame == null)
            {
                isValidFrame = frame =>
                    Protocol.Unpack(
                        frame,
                        out ulong serial,
                        out byte cmd,
                        out byte id,
                        out byte[] payload) == Protocol.ProtocolErrCode.Success;
            }

            // Dọn các frame cũ còn tồn đọng từ trước khi gửi lệnh mới, để
            // không nhầm lẫn với phản hồi của lần gửi này. Lưu ý: chỉ xóa
            // các frame ĐÃ HOÀN CHỈNH trong hàng đợi, KHÔNG đụng tới byte
            // đang gom dở (_rxAccumulator) — tránh cắt cụt 1 frame đang tới.
            lock (_rxLock)
            {
                _frameQueue.Clear();
            }

            DrainSignal();

            Send(txFrame);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                int remainingMs = timeoutMs - (int)stopwatch.ElapsedMilliseconds;

                if (remainingMs <= 0)
                {
                    return (false, null);
                }

                // Chờ tín hiệu "có frame mới" một cách bất đồng bộ, KHÔNG
                // chiếm thread trong lúc chờ (khác hẳn Thread.Sleep trước
                // đây). Nếu gọi hàm này bằng await từ UI thread, UI vẫn
                // phản hồi bình thường trong lúc chờ thiết bị trả lời.
                bool signaled = await _frameSignal
                    .WaitAsync(remainingMs, cancellationToken)
                    .ConfigureAwait(false);

                if (!signaled)
                {
                    // Hết thời gian chờ, không có thêm frame nào.
                    return (false, null);
                }

                if (Receive(out byte[] candidate))
                {
                    if (isValidFrame(candidate))
                    {
                        return (true, candidate);
                    }
                    // Không phải frame mong đợi (VD: frame "5A..." của MCU
                    // gửi tự do trên bus) → bỏ qua, vòng lặp tiếp tục chờ
                    // tín hiệu của frame kế tiếp.
                }
            }
        }

        private void SerialPort_DataReceived(
            object sender,
            SerialDataReceivedEventArgs e)
        {
            int count = _serialPort.BytesToRead;

            if (count <= 0)
            {
                return;
            }

            byte[] data = new byte[count];

            _serialPort.Read(
                data,
                0,
                count);

            lock (_rxLock)
            {
                _rxAccumulator.AddRange(data);
            }

            // Có data mới → reset khoảng lặng 100 ms
            _rxTimer.Change(
                RX_TIMEOUT_MS,
                Timeout.Infinite);
        }

        private void RxTimerCallback(object state)
        {
            byte[] frame;

            lock (_rxLock)
            {
                if (_rxAccumulator.Count == 0)
                {
                    return;
                }

                // Không có data mới trong 100 ms → coi như đã đủ 1 frame.
                frame = _rxAccumulator.ToArray();
                _rxAccumulator.Clear();

                if (_frameQueue.Count >= MAX_QUEUED_FRAMES)
                {
                    // Hàng đợi đầy do không ai lấy ra kịp (VD: bus có quá
                    // nhiều frame telemetry mà không có Communicate() nào
                    // đang chờ) → bỏ frame cũ nhất để tránh phình bộ nhớ.
                    _frameQueue.Dequeue();
                }

                _frameQueue.Enqueue(frame);
            }

            // Báo hiệu cho CommunicateAsync (nếu đang chờ) biết có frame mới
            _frameSignal.Release();

            // Bắn frame hoàn chỉnh cho Serial Monitor
            DataReceived?.Invoke(frame);
        }
    }
}