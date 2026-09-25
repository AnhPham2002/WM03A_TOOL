using System;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WM03A
{
    public partial class ucLogin : UserControl
    {
        private SerialPortManager _serialPortManager;

        private Protocol.AccessId _accessId;
        private byte[] _password;

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

            try
            {
                Protocol.AccessId accessId = (Protocol.AccessId)cmbRole.SelectedIndex;
                byte[] password = Encoding.ASCII.GetBytes(txtModulePassword.Text);

                bool success = await LoginAsync(accessId, password);

                if (success)
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
        }

        private async Task<bool> LoginAsync(Protocol.AccessId accessId, byte[] password)
        {
            byte[] txFrame;

            Protocol.Access(accessId, password, out txFrame);

            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

            if (!ok)
            {
                MessageBox.Show("Không nhận được phản hồi từ thiết bị.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Protocol.Unpack(rxFrame, out ulong serial, out byte cmd, out byte id, out byte[] payload);

            if (payload == null || payload.Length <= 6)
            {
                MessageBox.Show("Frame phản hồi không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (cmd != (byte)Protocol.CmdCode.Access || id != (byte)accessId)
            {
                return false;
            }

            return payload[6] == (byte)Protocol.ProtocolErrCode.Success;
        }

        public async Task<bool> ReconnectAsync()
        {
            if (!_serialPortManager.IsOpen)
            {
                return false;
            }

            if (_password == null)
            {
                return false;
            }

            return await LoginAsync(_accessId, _password);
        }
    }
}