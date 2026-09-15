using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace WM03A
{
    public class SerialPortManager
    {
        private const int RX_TIMEOUT_MS = 100;

        private SerialPort _serialPort;

        private readonly List<byte> _rxBuffer = new List<byte>();
        private readonly object _rxLock = new object();
        private readonly Timer _rxTimer;

        private bool _rxDataReady;

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
                _rxBuffer.Clear();
                _rxDataReady = false;
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

        public bool Receive(out byte[] data)
        {
            lock (_rxLock)
            {
                if (!_rxDataReady)
                {
                    data = null;
                    return false;
                }

                data = _rxBuffer.ToArray();

                _rxBuffer.Clear();

                _rxDataReady = false;

                return true;
            }
        }

        public bool Communicate(byte[] txFrame, out byte[] rxFrame)
        {
            rxFrame = null;

            Send(txFrame);

            const int timeoutMs = 300;
            const int pollingIntervalMs = 10;

            int elapsedMs = 0;

            while (elapsedMs < timeoutMs)
            {
                if (Receive(out rxFrame))
                {
                    return true;
                }

                Thread.Sleep(pollingIntervalMs);
                elapsedMs += pollingIntervalMs;
            }

            return false;
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
                _rxBuffer.AddRange(data);

                // Có data mới → chưa hoàn thành frame
                _rxDataReady = false;
            }

            // Có data mới → reset timeout 100 ms
            _rxTimer.Change(
                RX_TIMEOUT_MS,
                Timeout.Infinite);
        }

        private void RxTimerCallback(object state)
        {
            byte[] frame;

            lock (_rxLock)
            {
                if (_rxBuffer.Count == 0)
                {
                    return;
                }

                // Không có data mới trong 100 ms
                _rxDataReady = true;

                frame = _rxBuffer.ToArray();
            }

            // Bắn frame hoàn chỉnh cho Serial Monitor
            DataReceived?.Invoke(frame);
        }
    }
}