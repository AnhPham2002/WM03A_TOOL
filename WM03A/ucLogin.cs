using System;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace WM03A
{
    public partial class ucLogin : UserControl
    {
        private SerialPortManager _serialPortManager;
        public event EventHandler LoginSucceeded;
        public ucLogin()
        {
            InitializeComponent();
        }

        private void ucLogin_Load(object sender, EventArgs e)
        {
            _serialPortManager = new SerialPortManager();

            LoadComPorts();

            cmbRole.SelectedIndex = 0;
        }

        private void LoadComPorts()
        {
            cmbCom.Items.Clear();

            using (ManagementObjectSearcher searcher =
                   new ManagementObjectSearcher(
                       "SELECT Name FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"))
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

                    string portName = name.Substring(
                        start + 1,
                        end - start - 1);

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
                MessageBox.Show(
                    $"Không thể mở {port.PortName}.\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //LoginSucceeded?.Invoke(this, EventArgs.Empty);

            if (!_serialPortManager.IsOpen)
            {
                MessageBox.Show("COM chưa được mở.");
                return;
            }

            try
            {
                string data = "CONNECT\r\n";
                byte[] frame = Encoding.ASCII.GetBytes(data);

                _serialPortManager.Send(frame);

                // Sau này không chuyển màn hình ngay ở đây.
                // Chờ thiết bị phản hồi Login thành công.
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi khi gửi dữ liệu.\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
