using System;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WM03A.Protocol;

namespace WM03A
{
    public partial class ucLogin : UserControl
    {
        private SerialPortManager _serialPortManager;

        private Protocol.AccessId _accessId;
        private byte[] _password;
        private const int PROTOCOL_KEY_SIZE = 16;

        public event EventHandler<Protocol.AccessId> LoginSucceeded;

        public ucLogin(SerialPortManager serialPortManager)
        {
            InitializeComponent();

            _serialPortManager = serialPortManager;
        }

        private void ucLogin_Load(object sender, EventArgs e)
        {
            LoadComPorts();

            cmbRole.SelectedIndex = 3;
            txtModulePassword.Text = "33333333";
        }

        private void LoadComPorts()
        {
            cmbCom.Items.Clear();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"))
            {
                foreach (ManagementObject device in searcher.Get())
                {
                    string name = device["Name"]?.ToString();

                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    int start = name.LastIndexOf("(COM");
                    int end = name.IndexOf(")", start);

                    if (start < 0 || end < 0)
                    {
                        continue;
                    }

                    string portName = name.Substring(start + 1, end - start - 1);
                    string description = name.Substring(0, start).Trim();

                    cmbCom.Items.Add(new ComPortInfo
                    {
                        PortName = portName,
                        Description = description
                    });
                }
            }

            if (cmbCom.Items.Count > 0)
            {
                cmbCom.SelectedIndex = 0;
            }
        }

        private void btnRefreshCom_Click(object sender, EventArgs e)
        {
            LoadComPorts();
        }

        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (_serialPortManager.IsOpen)
            {
                _serialPortManager.Close();

                btnOpenCom.Text = "Mở COM";
                cmbCom.Enabled = true;
                btnRefreshCom.Enabled = true;

                return;
            }

            ComPortInfo port = cmbCom.SelectedItem as ComPortInfo;

            if (port == null)
            {
                MessageBox.Show("Vui lòng chọn COM.");
                return;
            }

            try
            {
                _serialPortManager.Open(port.PortName);

                btnOpenCom.Text = "Đóng COM";
                cmbCom.Enabled = false;
                btnRefreshCom.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở {port.PortName}.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (cmbRole.SelectedIndex == 0)
            {
                MessageBox.Show("Vui lòng chọn quyền đăng nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_serialPortManager.IsOpen)
            {
                MessageBox.Show("COM chưa được mở.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] password = Encoding.ASCII.GetBytes(txtModulePassword.Text);

            if (password.Length == 0 || password.Length > 32)
            {
                MessageBox.Show("Độ dài mật khẩu không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnLogin.Enabled = false;

                // Step 1: Handshake using Master Key
                byte[] handshakePayload = new byte[8];

                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(handshakePayload);
                }

                if (!await LoginAsync((Protocol.AccessId)0x00, handshakePayload))
                {
                    MessageBox.Show("Handshake thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 2: Authenticate using Session Key
                Protocol.AccessId accessId = (Protocol.AccessId)cmbRole.SelectedIndex;

                if (await LoginAsync(accessId, password))
                {
                    _accessId = accessId;
                    _password = (byte[])password.Clone();

                    LoginSucceeded?.Invoke(this, accessId);
                }
                else
                {
                    MessageBox.Show("Sai mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kết nối.\n{ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private async Task<bool> LoginAsync(Protocol.AccessId accessId, byte[] password)
        {
            bool isHandshake = (byte)accessId == 0x00;
            byte[] txPayload;
            byte[] txFrame;

            if (isHandshake)
            {
                txPayload = password;
            }
            else
            {
                if (password == null || password.Length == 0 || password.Length > 32)
                    return false;

                txPayload = new byte[password.Length + 1];
                txPayload[0] = (byte)password.Length;
                Buffer.BlockCopy(password, 0, txPayload, 1, password.Length);
            }

            bool packSuccess = isHandshake
                ? Protocol.PackMasterKey(true, Protocol.PROTOCOL_MODULE_SERIAL_COMMON, (byte)Protocol.CmdCode.Access, (byte)accessId, txPayload, out txFrame)
                : Protocol.Pack(true, Protocol.PROTOCOL_MODULE_SERIAL_COMMON, (byte)Protocol.CmdCode.Access, (byte)accessId, txPayload, out txFrame);

            if (!packSuccess)
                return false;

            Func<byte[], bool> isValidFrame = frame =>
            {
                ulong frameSerial;
                byte frameCmd;
                byte frameId;
                byte[] framePayload;

                Protocol.ProtocolErrCode result = isHandshake
                    ? Protocol.UnpackMasterKey(frame, out frameSerial, out frameCmd, out frameId, out framePayload)
                    : Protocol.Unpack(frame, out frameSerial, out frameCmd, out frameId, out framePayload);

                return result == Protocol.ProtocolErrCode.Success
                    && frameCmd == (byte)Protocol.CmdCode.Access
                    && frameId == (byte)accessId;
            };

            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300, isValidFrame);

            if (!ok)
            {
                MessageBox.Show("Không nhận được phản hồi từ thiết bị.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            ulong serial;
            byte cmd;
            byte id;
            byte[] payload;

            Protocol.ProtocolErrCode unpackResult = isHandshake
                ? Protocol.UnpackMasterKey(rxFrame, out serial, out cmd, out id, out payload)
                : Protocol.Unpack(rxFrame, out serial, out cmd, out id, out payload);

            if (unpackResult != Protocol.ProtocolErrCode.Success)
                return false;

            if (cmd != (byte)Protocol.CmdCode.Access || id != (byte)accessId || payload == null)
                return false;

            if (isHandshake)
            {
                if (payload.Length != 16)
                    return false;

                return Protocol.ProtocolSetKey(payload);
            }

            if (payload.Length <= 6)
                return false;

            return payload[6] == (byte)Protocol.ProtocolErrCode.Success;
        }

        public async Task<bool> ReconnectAsync()
        {
            if (!_serialPortManager.IsOpen || _password == null)
                return false;

            // Step 1: Generate a new random number and perform handshake
            byte[] handshakePayload = new byte[8];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(handshakePayload);
            }

            if (!await LoginAsync((Protocol.AccessId)0x00, handshakePayload))
                return false;

            // Step 2: Authenticate again using the new Session Key
            return await LoginAsync(_accessId, _password);
        }
    }
}