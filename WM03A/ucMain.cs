using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WM03A.Users.ProtocolCommands;
using WM03A.Users.ProtocolParser;
using static WM03A.Protocol;
using static WM03A.QueryParser;

namespace WM03A
{
    public partial class ucMain : UserControl
    {
        private Protocol.AccessId _accessId;

        public event EventHandler LogoutRequested;

        private SerialPortManager _serialPortManager;

        private bool _isReading = false;

        private const int DEVICE_TIME_HEIGHT_PC = 60;
        private const int DEVICE_TIME_HEIGHT_MANUAL = 90;

        public ucMain(SerialPortManager serialPortManager, Protocol.AccessId accessId)
        {
            InitializeComponent();

            _accessId = accessId;

            ApplyAccessControl();

            _serialPortManager = serialPortManager;
        }

        private void ucMain_Load(object sender, EventArgs e)
        {
            cmbWriteTimezoneSetting.SelectedIndex = 0;
            rdoPcTimeSetting.Checked = true;
            lblTimeSettingStatus.Text = string.Empty;
            lblModuleSettingStatus.Text = string.Empty;
            lblPulseMeter1SettingStatus.Text = string.Empty;
            lblPulseMeter2SettingStatus.Text = string.Empty;
            lblPulseMeter3SettingStatus.Text = string.Empty;
            lblPulseMeter4SettingStatus.Text = string.Empty;

            cmbPulseMeter1Pin1Setting.SelectedIndex = 0;
            cmbPulseMeter1Pin2Setting.SelectedIndex = 0;
            cmbPulseMeter1TypeSetting.SelectedIndex = 0;
            cmbPulseMeter1EdgeSetting.SelectedIndex = 0;

            cmbPulseMeter2Pin1Setting.SelectedIndex = 0;
            cmbPulseMeter2Pin2Setting.SelectedIndex = 0;
            cmbPulseMeter2TypeSetting.SelectedIndex = 0;
            cmbPulseMeter2EdgeSetting.SelectedIndex = 0;

            cmbPulseMeter3Pin1Setting.SelectedIndex = 0;
            cmbPulseMeter3Pin2Setting.SelectedIndex = 0;
            cmbPulseMeter3TypeSetting.SelectedIndex = 0;
            cmbPulseMeter3EdgeSetting.SelectedIndex = 0;

            cmbPulseMeter4Pin1Setting.SelectedIndex = 0;
            cmbPulseMeter4Pin2Setting.SelectedIndex = 0;
            cmbPulseMeter4TypeSetting.SelectedIndex = 0;
            cmbPulseMeter4EdgeSetting.SelectedIndex = 0;

            UpdatePulseMeter1Control();
            UpdatePulseMeter2Control();
            UpdatePulseMeter3Control();
            UpdatePulseMeter4Control();
        }

        private void ApplyAccessControl()
        {
            switch (_accessId)
            {
                case Protocol.AccessId.User:
                    break;

                case Protocol.AccessId.Technician:
                    break;

                case Protocol.AccessId.Admin:
                    break;
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        // ----------------------------------------------------------------------
        // Overall Tab
        // ----------------------------------------------------------------------

        private async void btnReadModuleInfoOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearModuleInfoOverall();

            _isReading = true;
            btnReadModuleInfoOverall.Enabled = false;

            try
            {
                await ReadModuleInfoOverall();
            }
            finally
            {
                _isReading = false;
                btnReadModuleInfoOverall.Enabled = true;
            }
        }

        private async void btnReadInternetStatusOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearInternetStatusOverall();

            _isReading = true;
            btnReadInternetStatusOverall.Enabled = false;

            try
            {
                await ReadInternetStatusOverall();
            }
            finally
            {
                _isReading = false;
                btnReadInternetStatusOverall.Enabled = true;
            }
        }

        private async void btnPulseMeterDataOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearPulseMeterDataOverall();

            _isReading = true;
            btnPulseMeterDataOverall.Enabled = false;

            try
            {
                await ReadPulseMeterDataOverall();
            }
            finally
            {
                _isReading = false;
                btnPulseMeterDataOverall.Enabled = true;
            }
        }

        private async void btnModbusMeterDataOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearModbusMeterOverall();

            _isReading = true;
            btnModbusMeterDataOverall.Enabled = false;

            try
            {
                await ReadModbusMeterDataOverall();
            }
            finally
            {
                _isReading = false;
                btnModbusMeterDataOverall.Enabled = true;
            }
        }

        private async void btnPressureSensorDataOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearPressureSensorOverall();

            _isReading = true;
            btnModbusMeterDataOverall.Enabled = false;

            try
            {
                await ReadPressureSensorDataOverall();
            }
            finally
            {
                _isReading = false;
                btnModbusMeterDataOverall.Enabled = true;
            }
        }

        private async void btnReadAllOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearModuleInfoOverall();
            ClearInternetStatusOverall();
            ClearPulseMeterDataOverall();
            ClearModbusMeterOverall();
            ClearPressureSensorOverall();

            _isReading = true;
            btnReadAllOverall.Enabled = false;

            try
            {
                await ReadModuleInfoOverall();
                await ReadInternetStatusOverall();
                await ReadPulseMeterDataOverall();
                await ReadModbusMeterDataOverall();
                await ReadPressureSensorDataOverall();
            }
            finally
            {
                _isReading = false;
                btnReadAllOverall.Enabled = true;
            }
        }

        private void btnClearOverall_Click(object sender, EventArgs e)
        {
            if (_isReading)
            {
                MessageBox.Show("Đang đọc dữ liệu, vui lòng chờ.");
                return;
            }

            ClearModuleInfoOverall();
            ClearInternetStatusOverall();
            ClearPulseMeterDataOverall();
            ClearModbusMeterOverall();
            ClearPressureSensorOverall();
        }

        // ----------------------------------------------------------------------
        // Setting Tab
        // ----------------------------------------------------------------------

        //-----------------------Module Setting--------------------------------//
        private void rdoManualTimeSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoManualTimeSetting.Checked)
            {
                return;
            }

            grpTimeSetting.Height = DEVICE_TIME_HEIGHT_MANUAL;
            dtpWriteTimeSetting.Visible = true;
        }

        private void rdoPcTimeSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!rdoPcTimeSetting.Checked)
            {
                return;
            }

            grpTimeSetting.Height = DEVICE_TIME_HEIGHT_PC;
            dtpWriteTimeSetting.Visible = false;
        }

        private async void btnReadTimeSetting_Click(object sender, EventArgs e)
        {
            txtReadTimeSetting.Clear();
            GetCommands.DateTime(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok && GetParser.DateTime(rxFrame, out DateTime dateTime))
            {
                txtReadTimeSetting.Text = dateTime.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        private async void btnWriteTimeSetting_Click(object sender, EventArgs e)
        {
            DateTime dateTime;
            if (rdoPcTimeSetting.Checked)
            {
                dateTime = DateTime.Now;
            }
            else
            {
                dateTime = dtpWriteTimeSetting.Value;
            }

            SetCommands.DateTime(dateTime, out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);

            if (ok && SetParser.DateTime(rxFrame))
            {
                lblTimeSettingStatus.Text = "Ghi thành công";
                await Task.Delay(1000);
                lblTimeSettingStatus.Text = string.Empty;
                return;
            }

            lblTimeSettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblTimeSettingStatus.Text = string.Empty;
            return;
        }

        private async void btnReadModuleSetting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            txtReadIpSetting.Clear();
            txtReadPortSetting.Clear();
            txtReadLatchSetting.Clear();
            txtReadPushSetting.Clear();
            txtReadTimezoneSetting.Clear();

            // Read IP Endpoint
            GetCommands.IpEndpoint(out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok && GetParser.IpEndpoint(rxFrame, out string ip, out string port))
            {
                txtReadIpSetting.Text = ip;
                txtReadPortSetting.Text = port;
            }

            // Read Latch Period
            GetCommands.LatchPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                ushort latchPeriod = GetParser.LatchPeriod(rxFrame);
                txtReadLatchSetting.Text = latchPeriod.ToString();
            }

            // Read Push Period
            GetCommands.PushPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                ushort PushPeriod = GetParser.PushPeriod(rxFrame);
                txtReadPushSetting.Text = PushPeriod.ToString();
            }

            // Read Timezone
            GetCommands.Timezone(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                float Timezone = GetParser.Timezone(rxFrame);
                txtReadTimezoneSetting.Text = GetTimezoneText(Timezone);
            }
        }

        private void txtWriteIpSetting_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }

            if (!char.IsControl(e.KeyChar) && textBox.Text.Length >= 15 && textBox.SelectionLength == 0)
            {
                e.Handled = true;
            }
        }

        private void txtWritePortSetting_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (!char.IsControl(e.KeyChar))
            {
                string newText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
                newText = newText.Insert(textBox.SelectionStart, e.KeyChar.ToString());

                if (!ushort.TryParse(newText, out ushort value) || value > 65535)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtWriteLatchSetting_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtWritePushSetting_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cboWriteTimezoneSetting_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                cmbWriteTimezoneSetting.SelectedIndex = 0;
                e.Handled = true;
            }
        }

        private async void btnWriteModuleSetting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool writeStatus = true;

            if (!string.IsNullOrWhiteSpace(txtWriteIpSetting.Text) != !string.IsNullOrWhiteSpace(txtWritePortSetting.Text))
            {
                lblModuleSettingStatus.Text = "Vui lòng nhập đầy đủ IP và Port, hoặc xóa cả hai trường.";
                return;
            }

            if (!string.IsNullOrWhiteSpace(txtWriteLatchSetting.Text))
            {
                if (!ushort.TryParse(txtWriteLatchSetting.Text, out ushort value) || value < 1 || value > 2440)
                {
                    lblModuleSettingStatus.Text = "Chu kỳ chốt phải nằm trong khoảng từ 1 đến 2440.";
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtWritePushSetting.Text))
            {
                if (!ushort.TryParse(txtWritePushSetting.Text, out ushort value) || value < 1 || value > 2440)
                {
                    lblModuleSettingStatus.Text = "Chu kỳ đẩy phải nằm trong khoảng từ 1 đến 2440.";
                    return;
                }
            }

            if (!TryGetTimezone(out string timezone))
            {
                lblModuleSettingStatus.Text = "Múi giờ không hợp lệ.";
                return;
            }

            if (SetCommands.IpEndpoint(txtWriteIpSetting.Text, txtWritePortSetting.Text, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
                if (!ok || !SetParser.IpEndpoint(rxFrame))
                {
                    writeStatus = false;
                }
            }

            if (SetCommands.ModuleConfig(txtWriteLatchSetting.Text, txtWritePushSetting.Text, timezone, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
                if (!ok || !SetParser.ModuleConfig(rxFrame))
                {
                    writeStatus = false;
                }
            }

            if (writeStatus)
            {
                lblModuleSettingStatus.Text = "Ghi thành công";
                await Task.Delay(1000);
                lblModuleSettingStatus.Text = string.Empty;
                return;
            }

            lblModuleSettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblModuleSettingStatus.Text = string.Empty;
            return;
        }

        //-----------------------Pulse Meter Setting--------------------------------//

        private async void btnReadPulseMeter1Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
                (byte)ConfigPulseMeterId.MeterEnable,
                (byte)ConfigPulseMeterId.SerialNumber,
                (byte)ConfigPulseMeterId.PulseFactor,
                (byte)ConfigPulseMeterId.Pin1,
                (byte)ConfigPulseMeterId.Pin2,
                (byte)ConfigPulseMeterId.PulseType,
                (byte)ConfigPulseMeterId.EdgeType
            };

            txtReadPulseMeter1UseSetting.Clear();
            txtReadPulseMeter1SerialSetting.Clear();
            txtReadPulseMeter1PulseFactorSetting.Clear();
            txtReadPulseMeter1TypeSetting.Clear();
            txtReadPulseMeter1Pin1Setting.Clear();
            txtReadPulseMeter1Pin2Setting.Clear();
            txtReadPulseMeter1EdgeSetting.Clear();
            txtReadPulseMeter1ForwardSetting.Clear();
            txtReadPulseMeter1ReverseSetting.Clear();

            GetCommands.PulseMeter(0, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && GetParser.PulseMeter(rxFrame, out PulseMeterConfig pulseMeterConfig))
            {
                if (!pulseMeterConfig.MeterEnable)
                {
                    txtReadPulseMeter1UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadPulseMeter1UseSetting.Text = "Đang sử dụng";
                    txtReadPulseMeter1SerialSetting.Text = pulseMeterConfig.SerialNumber;
                    txtReadPulseMeter1PulseFactorSetting.Text = pulseMeterConfig.PulseFactor.ToString();
                    if (pulseMeterConfig.PulseType != 0)
                    {
                        txtReadPulseMeter1TypeSetting.Text = cmbPulseMeter1TypeSetting.Items[pulseMeterConfig.PulseType].ToString();
                    }
                    if (pulseMeterConfig.Pin1 != 0)
                    {
                        txtReadPulseMeter1Pin1Setting.Text = cmbPulseMeter1Pin1Setting.Items[pulseMeterConfig.Pin1].ToString();
                    }
                    if (pulseMeterConfig.Pin2 != 0)
                    {
                        txtReadPulseMeter1Pin2Setting.Text = cmbPulseMeter1Pin2Setting.Items[pulseMeterConfig.Pin2].ToString();
                    }
                    if (pulseMeterConfig.EdgeType != 0)
                    {
                        txtReadPulseMeter1EdgeSetting.Text = cmbPulseMeter1EdgeSetting.Items[pulseMeterConfig.EdgeType].ToString();
                    }
                }
            }

            QueryCommands.PulseMeterData(0, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter1ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter1ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
        }

        private void chkPulseMeter1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter1Control();
        }

        private void cmbPulseMeter1TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter1Control();
        }

        private async void btnWritePulseMeter1Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkPulseMeter1UseSetting.Checked;

            string serialNumber = txtWritePulseMeter1SerialSetting.Text.Trim();
            ushort? pulseFactor = null;
            double? forward = null;
            double? reverse = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblPulseMeter1SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!ushort.TryParse(txtWritePulseMeter1PulseFactorSetting.Text, out ushort pulseFactorValue) || pulseFactorValue < 1)
                {
                    lblPulseMeter1SettingStatus.Text = "Pulse Factor phải lớn hơn hoặc bằng 1.";
                    return;
                }

                if (cmbPulseMeter1TypeSetting.SelectedIndex == 0 || cmbPulseMeter1Pin1Setting.SelectedIndex == 0 || cmbPulseMeter1EdgeSetting.SelectedIndex == 0)
                {
                    lblPulseMeter1SettingStatus.Text = "Vui lòng điền đầy đủ thông tin.";
                    return;
                }

                if (cmbPulseMeter1TypeSetting.SelectedIndex == 2 && cmbPulseMeter1Pin2Setting.SelectedIndex == 0)
                {
                    lblPulseMeter1SettingStatus.Text = "Vui lòng chọn Pin 2.";
                    return;
                }

                string forwardText = txtWritePulseMeter1ForwardSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(forwardText, out double forwardValue) || forwardValue < 0)
                {
                    lblPulseMeter1SettingStatus.Text = "Forward phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                string reverseText = txtWritePulseMeter1ReverseSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(reverseText, out double reverseValue) || reverseValue < 0)
                {
                    lblPulseMeter1SettingStatus.Text = "Reverse phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                pulseFactor = pulseFactorValue;
                forward = forwardValue;
                reverse = reverseValue;
            }

            byte pulseType = (byte)cmbPulseMeter1TypeSetting.SelectedIndex;
            byte pin1 = (byte)cmbPulseMeter1Pin1Setting.SelectedIndex;
            byte pin2 = (byte)cmbPulseMeter1Pin2Setting.SelectedIndex;
            byte edgeType = (byte)cmbPulseMeter1EdgeSetting.SelectedIndex;

            if (SetCommands.PulseMeter(0, enabled, serialNumber, pulseFactor, pulseType, pin1, pin2, edgeType, forward, reverse, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);

                if (ok && SetParser.PulseMeter(rxFrame))
                {
                    lblPulseMeter1SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblPulseMeter1SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblPulseMeter1SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblPulseMeter1SettingStatus.Text = string.Empty;
        }




        private async void btnReadPulseMeter2Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
                (byte)ConfigPulseMeterId.MeterEnable,
                (byte)ConfigPulseMeterId.SerialNumber,
                (byte)ConfigPulseMeterId.PulseFactor,
                (byte)ConfigPulseMeterId.Pin1,
                (byte)ConfigPulseMeterId.Pin2,
                (byte)ConfigPulseMeterId.PulseType,
                (byte)ConfigPulseMeterId.EdgeType
            };

            txtReadPulseMeter2UseSetting.Clear();
            txtReadPulseMeter2SerialSetting.Clear();
            txtReadPulseMeter2PulseFactorSetting.Clear();
            txtReadPulseMeter2TypeSetting.Clear();
            txtReadPulseMeter2Pin1Setting.Clear();
            txtReadPulseMeter2Pin2Setting.Clear();
            txtReadPulseMeter2EdgeSetting.Clear();
            txtReadPulseMeter2ForwardSetting.Clear();
            txtReadPulseMeter2ReverseSetting.Clear();

            GetCommands.PulseMeter(1, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && GetParser.PulseMeter(rxFrame, out PulseMeterConfig pulseMeterConfig))
            {
                if (!pulseMeterConfig.MeterEnable)
                {
                    txtReadPulseMeter2UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadPulseMeter2UseSetting.Text = "Đang sử dụng";
                    txtReadPulseMeter2SerialSetting.Text = pulseMeterConfig.SerialNumber;
                    txtReadPulseMeter2PulseFactorSetting.Text = pulseMeterConfig.PulseFactor.ToString();
                    if (pulseMeterConfig.PulseType != 0)
                    {
                        txtReadPulseMeter2TypeSetting.Text = cmbPulseMeter2TypeSetting.Items[pulseMeterConfig.PulseType].ToString();
                    }
                    if (pulseMeterConfig.Pin1 != 0)
                    {
                        txtReadPulseMeter2Pin1Setting.Text = cmbPulseMeter2Pin1Setting.Items[pulseMeterConfig.Pin1].ToString();
                    }
                    if (pulseMeterConfig.Pin2 != 0)
                    {
                        txtReadPulseMeter2Pin2Setting.Text = cmbPulseMeter2Pin2Setting.Items[pulseMeterConfig.Pin2].ToString();
                    }
                    if (pulseMeterConfig.EdgeType != 0)
                    {
                        txtReadPulseMeter2EdgeSetting.Text = cmbPulseMeter2EdgeSetting.Items[pulseMeterConfig.EdgeType].ToString();
                    }
                }
            }

            QueryCommands.PulseMeterData(1, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter2ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter2ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
        }
        private void chkPulseMeter2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter2Control();
        }

        private void cmbPulseMeter2TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter2Control();
        }

        private async void btnWritePulseMeter2Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkPulseMeter2UseSetting.Checked;

            string serialNumber = txtWritePulseMeter2SerialSetting.Text.Trim();
            ushort? pulseFactor = null;
            double? forward = null;
            double? reverse = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblPulseMeter2SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!ushort.TryParse(txtWritePulseMeter2PulseFactorSetting.Text, out ushort pulseFactorValue) || pulseFactorValue < 1)
                {
                    lblPulseMeter2SettingStatus.Text = "Pulse Factor phải lớn hơn hoặc bằng 1.";
                    return;
                }

                if (cmbPulseMeter2TypeSetting.SelectedIndex == 0 || cmbPulseMeter2Pin1Setting.SelectedIndex == 0 || cmbPulseMeter2EdgeSetting.SelectedIndex == 0)
                {
                    lblPulseMeter2SettingStatus.Text = "Vui lòng điền đầy đủ thông tin.";
                    return;
                }

                if (cmbPulseMeter2TypeSetting.SelectedIndex == 2 && cmbPulseMeter2Pin2Setting.SelectedIndex == 0)
                {
                    lblPulseMeter2SettingStatus.Text = "Vui lòng chọn Pin 2.";
                    return;
                }

                string forwardText = txtWritePulseMeter2ForwardSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(forwardText, out double forwardValue) || forwardValue < 0)
                {
                    lblPulseMeter2SettingStatus.Text = "Forward phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                string reverseText = txtWritePulseMeter2ReverseSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(reverseText, out double reverseValue) || reverseValue < 0)
                {
                    lblPulseMeter2SettingStatus.Text = "Reverse phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                pulseFactor = pulseFactorValue;
                forward = forwardValue;
                reverse = reverseValue;
            }

            byte pulseType = (byte)cmbPulseMeter2TypeSetting.SelectedIndex;
            byte pin1 = (byte)cmbPulseMeter2Pin1Setting.SelectedIndex;
            byte pin2 = (byte)cmbPulseMeter2Pin2Setting.SelectedIndex;
            byte edgeType = (byte)cmbPulseMeter2EdgeSetting.SelectedIndex;

            if (SetCommands.PulseMeter(1, enabled, serialNumber, pulseFactor, pulseType, pin1, pin2, edgeType, forward, reverse, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);

                if (ok && SetParser.PulseMeter(rxFrame))
                {
                    lblPulseMeter2SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblPulseMeter2SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblPulseMeter2SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblPulseMeter2SettingStatus.Text = string.Empty;
        }

        private async void btnReadPulseMeter3Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
                (byte)ConfigPulseMeterId.MeterEnable,
                (byte)ConfigPulseMeterId.SerialNumber,
                (byte)ConfigPulseMeterId.PulseFactor,
                (byte)ConfigPulseMeterId.Pin1,
                (byte)ConfigPulseMeterId.Pin2,
                (byte)ConfigPulseMeterId.PulseType,
                (byte)ConfigPulseMeterId.EdgeType
            };

            txtReadPulseMeter3UseSetting.Clear();
            txtReadPulseMeter3SerialSetting.Clear();
            txtReadPulseMeter3PulseFactorSetting.Clear();
            txtReadPulseMeter3TypeSetting.Clear();
            txtReadPulseMeter3Pin1Setting.Clear();
            txtReadPulseMeter3Pin2Setting.Clear();
            txtReadPulseMeter3EdgeSetting.Clear();
            txtReadPulseMeter3ForwardSetting.Clear();
            txtReadPulseMeter3ReverseSetting.Clear();

            GetCommands.PulseMeter(2, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && GetParser.PulseMeter(rxFrame, out PulseMeterConfig pulseMeterConfig))
            {
                if (!pulseMeterConfig.MeterEnable)
                {
                    txtReadPulseMeter3UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadPulseMeter3UseSetting.Text = "Đang sử dụng";
                    txtReadPulseMeter3SerialSetting.Text = pulseMeterConfig.SerialNumber;
                    txtReadPulseMeter3PulseFactorSetting.Text = pulseMeterConfig.PulseFactor.ToString();
                    if (pulseMeterConfig.PulseType != 0)
                    {
                        txtReadPulseMeter3TypeSetting.Text = cmbPulseMeter3TypeSetting.Items[pulseMeterConfig.PulseType].ToString();
                    }
                    if (pulseMeterConfig.Pin1 != 0)
                    {
                        txtReadPulseMeter3Pin1Setting.Text = cmbPulseMeter3Pin1Setting.Items[pulseMeterConfig.Pin1].ToString();
                    }
                    if (pulseMeterConfig.Pin2 != 0)
                    {
                        txtReadPulseMeter3Pin2Setting.Text = cmbPulseMeter3Pin2Setting.Items[pulseMeterConfig.Pin2].ToString();
                    }
                    if (pulseMeterConfig.EdgeType != 0)
                    {
                        txtReadPulseMeter3EdgeSetting.Text = cmbPulseMeter3EdgeSetting.Items[pulseMeterConfig.EdgeType].ToString();
                    }
                }
            }

            QueryCommands.PulseMeterData(2, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter3ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter3ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
        }

        private void chkPulseMeter3UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter3Control();
        }

        private void cmbPulseMeter3TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter3Control();
        }

        private async void btnWritePulseMeter3Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkPulseMeter3UseSetting.Checked;

            string serialNumber = txtWritePulseMeter3SerialSetting.Text.Trim();
            ushort? pulseFactor = null;
            double? forward = null;
            double? reverse = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblPulseMeter3SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!ushort.TryParse(txtWritePulseMeter3PulseFactorSetting.Text, out ushort pulseFactorValue) || pulseFactorValue < 1)
                {
                    lblPulseMeter3SettingStatus.Text = "Pulse Factor phải lớn hơn hoặc bằng 1.";
                    return;
                }

                if (cmbPulseMeter3TypeSetting.SelectedIndex == 0 || cmbPulseMeter3Pin1Setting.SelectedIndex == 0 || cmbPulseMeter3EdgeSetting.SelectedIndex == 0)
                {
                    lblPulseMeter3SettingStatus.Text = "Vui lòng điền đầy đủ thông tin.";
                    return;
                }

                if (cmbPulseMeter3TypeSetting.SelectedIndex == 2 && cmbPulseMeter3Pin2Setting.SelectedIndex == 0)
                {
                    lblPulseMeter3SettingStatus.Text = "Vui lòng chọn Pin 2.";
                    return;
                }

                string forwardText = txtWritePulseMeter3ForwardSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(forwardText, out double forwardValue) || forwardValue < 0)
                {
                    lblPulseMeter3SettingStatus.Text = "Forward phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                string reverseText = txtWritePulseMeter3ReverseSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(reverseText, out double reverseValue) || reverseValue < 0)
                {
                    lblPulseMeter3SettingStatus.Text = "Reverse phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                pulseFactor = pulseFactorValue;
                forward = forwardValue;
                reverse = reverseValue;
            }

            byte pulseType = (byte)cmbPulseMeter3TypeSetting.SelectedIndex;
            byte pin1 = (byte)cmbPulseMeter3Pin1Setting.SelectedIndex;
            byte pin2 = (byte)cmbPulseMeter3Pin2Setting.SelectedIndex;
            byte edgeType = (byte)cmbPulseMeter3EdgeSetting.SelectedIndex;

            if (SetCommands.PulseMeter(2, enabled, serialNumber, pulseFactor, pulseType, pin1, pin2, edgeType, forward, reverse, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);

                if (ok && SetParser.PulseMeter(rxFrame))
                {
                    lblPulseMeter3SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblPulseMeter3SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblPulseMeter3SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblPulseMeter3SettingStatus.Text = string.Empty;
        }


        private async void btnReadPulseMeter4Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
                (byte)ConfigPulseMeterId.MeterEnable,
                (byte)ConfigPulseMeterId.SerialNumber,
                (byte)ConfigPulseMeterId.PulseFactor,
                (byte)ConfigPulseMeterId.Pin1,
                (byte)ConfigPulseMeterId.Pin2,
                (byte)ConfigPulseMeterId.PulseType,
                (byte)ConfigPulseMeterId.EdgeType
            };

            txtReadPulseMeter4UseSetting.Clear();
            txtReadPulseMeter4SerialSetting.Clear();
            txtReadPulseMeter4PulseFactorSetting.Clear();
            txtReadPulseMeter4TypeSetting.Clear();
            txtReadPulseMeter4Pin1Setting.Clear();
            txtReadPulseMeter4Pin2Setting.Clear();
            txtReadPulseMeter4EdgeSetting.Clear();
            txtReadPulseMeter4ForwardSetting.Clear();
            txtReadPulseMeter4ReverseSetting.Clear();

            GetCommands.PulseMeter(3, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && GetParser.PulseMeter(rxFrame, out PulseMeterConfig pulseMeterConfig))
            {
                if (!pulseMeterConfig.MeterEnable)
                {
                    txtReadPulseMeter4UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadPulseMeter4UseSetting.Text = "Đang sử dụng";
                    txtReadPulseMeter4SerialSetting.Text = pulseMeterConfig.SerialNumber;
                    txtReadPulseMeter4PulseFactorSetting.Text = pulseMeterConfig.PulseFactor.ToString();
                    if (pulseMeterConfig.PulseType != 0)
                    {
                        txtReadPulseMeter4TypeSetting.Text = cmbPulseMeter4TypeSetting.Items[pulseMeterConfig.PulseType].ToString();
                    }
                    if (pulseMeterConfig.Pin1 != 0)
                    {
                        txtReadPulseMeter4Pin1Setting.Text = cmbPulseMeter4Pin1Setting.Items[pulseMeterConfig.Pin1].ToString();
                    }
                    if (pulseMeterConfig.Pin2 != 0)
                    {
                        txtReadPulseMeter4Pin2Setting.Text = cmbPulseMeter4Pin2Setting.Items[pulseMeterConfig.Pin2].ToString();
                    }
                    if (pulseMeterConfig.EdgeType != 0)
                    {
                        txtReadPulseMeter4EdgeSetting.Text = cmbPulseMeter4EdgeSetting.Items[pulseMeterConfig.EdgeType].ToString();
                    }
                }
            }

            QueryCommands.PulseMeterData(3, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter4ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter4ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
        }

        private void chkPulseMeter4UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter4Control();
        }

        private void cmbPulseMeter4TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter4Control();
        }
        private async void btnWritePulseMeter4Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkPulseMeter4UseSetting.Checked;

            string serialNumber = txtWritePulseMeter4SerialSetting.Text.Trim();
            ushort? pulseFactor = null;
            double? forward = null;
            double? reverse = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblPulseMeter4SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!ushort.TryParse(txtWritePulseMeter4PulseFactorSetting.Text, out ushort pulseFactorValue) || pulseFactorValue < 1)
                {
                    lblPulseMeter4SettingStatus.Text = "Pulse Factor phải lớn hơn hoặc bằng 1.";
                    return;
                }

                if (cmbPulseMeter4TypeSetting.SelectedIndex == 0 || cmbPulseMeter4Pin1Setting.SelectedIndex == 0 || cmbPulseMeter4EdgeSetting.SelectedIndex == 0)
                {
                    lblPulseMeter4SettingStatus.Text = "Vui lòng điền đầy đủ thông tin.";
                    return;
                }

                if (cmbPulseMeter4TypeSetting.SelectedIndex == 2 && cmbPulseMeter4Pin2Setting.SelectedIndex == 0)
                {
                    lblPulseMeter4SettingStatus.Text = "Vui lòng chọn Pin 2.";
                    return;
                }

                string forwardText = txtWritePulseMeter4ForwardSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(forwardText, out double forwardValue) || forwardValue < 0)
                {
                    lblPulseMeter4SettingStatus.Text = "Forward phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                string reverseText = txtWritePulseMeter4ReverseSetting.Text.Trim().Replace('.', ',');
                if (!double.TryParse(reverseText, out double reverseValue) || reverseValue < 0)
                {
                    lblPulseMeter4SettingStatus.Text = "Reverse phải là số lớn hơn hoặc bằng 0.";
                    return;
                }

                pulseFactor = pulseFactorValue;
                forward = forwardValue;
                reverse = reverseValue;
            }

            byte pulseType = (byte)cmbPulseMeter4TypeSetting.SelectedIndex;
            byte pin1 = (byte)cmbPulseMeter4Pin1Setting.SelectedIndex;
            byte pin2 = (byte)cmbPulseMeter4Pin2Setting.SelectedIndex;
            byte edgeType = (byte)cmbPulseMeter4EdgeSetting.SelectedIndex;

            if (SetCommands.PulseMeter(3, enabled, serialNumber, pulseFactor, pulseType, pin1, pin2, edgeType, forward, reverse, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);

                if (ok && SetParser.PulseMeter(rxFrame))
                {
                    lblPulseMeter4SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblPulseMeter4SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblPulseMeter4SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblPulseMeter4SettingStatus.Text = string.Empty;
        }


        // ----------------------------------------------------------------------
        // Logic functions
        // ----------------------------------------------------------------------

        private void ClearModuleInfoOverall()
        {
            txtModuleSerialOverall.Clear();
            txtModuleActivationStatus.Clear();
            txtBootloaderVersion.Clear();
            txtFirmwareVersion.Clear();
            txtTimeOverall.Clear();
            txtIPV4Overall.Clear();
            txtPortOverall.Clear();

            txtLatchPeriodOverall.Clear();
            txtPushPeriodOverall.Clear();
            txtTimezoneOverall.Clear();
            txtPowerStatus.Clear();
            txtChargeStatus.Clear();
            txtBatteryVoltage.Clear();
        }
        private async Task ReadModuleInfoOverall()
        {
            byte[] txFrame;

            // Read Module Serial
            GetCommands.ModuleSerial(out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                ulong serial = GetParser.ModuleSerial(rxFrame);
                txtModuleSerialOverall.Text = serial.ToString("D12");
            }

            // Read Date Time
            GetCommands.DateTime(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok && GetParser.DateTime(rxFrame, out DateTime dateTime))
            {
                txtTimeOverall.Text = dateTime.ToString("dd/MM/yyyy HH:mm:ss");
            }

            // Read IP Endpoint
            GetCommands.IpEndpoint(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok && GetParser.IpEndpoint(rxFrame, out string ip, out string port))
            {
                txtIPV4Overall.Text = ip;
                txtPortOverall.Text = port;
            }

            // Read Latch Period
            GetCommands.LatchPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                ushort latchPeriod = GetParser.LatchPeriod(rxFrame);
                txtLatchPeriodOverall.Text = latchPeriod.ToString();
            }

            // Read Push Period
            GetCommands.PushPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                ushort PushPeriod = GetParser.PushPeriod(rxFrame);
                txtPushPeriodOverall.Text = PushPeriod.ToString();
            }

            // Read Timezone
            GetCommands.Timezone(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 300);
            if (ok)
            {
                float Timezone = GetParser.Timezone(rxFrame);
                txtTimezoneOverall.Text = GetTimezoneText(Timezone);
            }
        }

        private void ClearInternetStatusOverall()
        {
            txtSimCcid.Clear();
            txtRssi.Clear();
            txtRsrp.Clear();
            txtRsrq.Clear();
            txtRssnr.Clear();
        }

        private async Task ReadInternetStatusOverall()
        {
            byte[] txFrame;
            byte[] ccid;
            sbyte rssi;
            sbyte rsrp;
            sbyte rsrq;
            sbyte rssnr;

            QueryCommands.SimNetworkInfo(out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 20000);
            if (ok && QueryParser.SimNetworkInfo(rxFrame, out ccid, out rssi, out rsrp, out rsrq, out rssnr))
            {
                txtSimCcid.Text = Encoding.ASCII.GetString(ccid).TrimEnd('\0');
                txtRssi.Text = rssi.ToString();
                txtRsrp.Text = rsrp.ToString();
                txtRsrq.Text = rsrq.ToString();
                txtRssnr.Text = rssnr.ToString();
            }
        }

        private void ClearPulseMeterDataOverall()
        {
            txtPulseMeterSerial1Overall.Clear();
            txtPulseMeterForward1Overall.Clear();
            txtPulseMeterReverse1Overall.Clear();
            txtPulseMeterRealTotal1Overall.Clear();
            txtPulseMeterFlowRate1Overall.Clear();

            txtPulseMeterSerial2Overall.Clear();
            txtPulseMeterForward2Overall.Clear();
            txtPulseMeterReverse2Overall.Clear();
            txtPulseMeterRealTotal2Overall.Clear();
            txtPulseMeterFlowRate2Overall.Clear();

            txtPulseMeterSerial3Overall.Clear();
            txtPulseMeterForward3Overall.Clear();
            txtPulseMeterReverse3Overall.Clear();
            txtPulseMeterRealTotal3Overall.Clear();
            txtPulseMeterFlowRate3Overall.Clear();

            txtPulseMeterSerial4Overall.Clear();
            txtPulseMeterForward4Overall.Clear();
            txtPulseMeterReverse4Overall.Clear();
            txtPulseMeterRealTotal4Overall.Clear();
            txtPulseMeterFlowRate4Overall.Clear();
        }

        private async Task ReadPulseMeterDataOverall()
        {
            byte[] txFrame;
            QueryParser.MeterData meterData;

            QueryCommands.PulseMeterData(0, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial1Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward1Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse1Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal1Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate1Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.PulseMeterData(1, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial2Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward2Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse2Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal2Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate2Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.PulseMeterData(2, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial3Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward3Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse3Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal3Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate3Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.PulseMeterData(3, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 500);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial4Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward4Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse4Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal4Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate4Overall.Text = meterData.FlowRate.ToString();
            }
        }

        private void ClearModbusMeterOverall()
        {
            txtModbusMeterSerial1Overall.Clear();
            txtModbusMeterForward1Overall.Clear();
            txtModbusMeterReverse1Overall.Clear();
            txtModbusMeterRealTotal1Overall.Clear();
            txtModbusMeterFlowRate1Overall.Clear();

            txtModbusMeterSerial2Overall.Clear();
            txtModbusMeterForward2Overall.Clear();
            txtModbusMeterReverse2Overall.Clear();
            txtModbusMeterRealTotal2Overall.Clear();
            txtModbusMeterFlowRate2Overall.Clear();

            txtModbusMeterSerial3Overall.Clear();
            txtModbusMeterForward3Overall.Clear();
            txtModbusMeterReverse3Overall.Clear();
            txtModbusMeterRealTotal3Overall.Clear();
            txtModbusMeterFlowRate3Overall.Clear();

            txtModbusMeterSerial4Overall.Clear();
            txtModbusMeterForward4Overall.Clear();
            txtModbusMeterReverse4Overall.Clear();
            txtModbusMeterRealTotal4Overall.Clear();
            txtModbusMeterFlowRate4Overall.Clear();
        }

        private async Task ReadModbusMeterDataOverall()
        {
            byte[] txFrame;
            QueryParser.MeterData meterData = new QueryParser.MeterData();

            QueryCommands.ModbusMeterData(0, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.ModbusMeterData(rxFrame, out meterData))
            {
                txtModbusMeterSerial1Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtModbusMeterForward1Overall.Text = meterData.ForwardTotalizer.ToString();
                txtModbusMeterReverse1Overall.Text = meterData.ReverseTotalizer.ToString();
                txtModbusMeterRealTotal1Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtModbusMeterFlowRate1Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.ModbusMeterData(1, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.ModbusMeterData(rxFrame, out meterData))
            {
                txtModbusMeterSerial2Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtModbusMeterForward2Overall.Text = meterData.ForwardTotalizer.ToString();
                txtModbusMeterReverse2Overall.Text = meterData.ReverseTotalizer.ToString();
                txtModbusMeterRealTotal2Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtModbusMeterFlowRate2Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.ModbusMeterData(2, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.ModbusMeterData(rxFrame, out meterData))
            {
                txtModbusMeterSerial3Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtModbusMeterForward3Overall.Text = meterData.ForwardTotalizer.ToString();
                txtModbusMeterReverse3Overall.Text = meterData.ReverseTotalizer.ToString();
                txtModbusMeterRealTotal3Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtModbusMeterFlowRate3Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.ModbusMeterData(3, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.ModbusMeterData(rxFrame, out meterData))
            {
                txtModbusMeterSerial4Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtModbusMeterForward4Overall.Text = meterData.ForwardTotalizer.ToString();
                txtModbusMeterReverse4Overall.Text = meterData.ReverseTotalizer.ToString();
                txtModbusMeterRealTotal4Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtModbusMeterFlowRate4Overall.Text = meterData.FlowRate.ToString();
            }
        }

        private void ClearPressureSensorOverall()
        {
            txtPressureSensorSerial1Overall.Clear();
            txtPressure1Overall.Clear();

            txtPressureSensorSerial2Overall.Clear();
            txtPressure2Overall.Clear();
        }

        private async Task ReadPressureSensorDataOverall()
        {
            byte[] txFrame;
            byte[] meterSerial;
            float pressure;

            QueryCommands.PressureSensorData(0, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 1000);
            if (ok && QueryParser.PressureSensorData(rxFrame, out meterSerial, out pressure))
            {
                txtPressureSensorSerial1Overall.Text = Encoding.ASCII.GetString(meterSerial).TrimEnd('\0');
                txtPressure1Overall.Text = pressure.ToString();
            }

            QueryCommands.PressureSensorData(1, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 1000);
            if (ok && QueryParser.PressureSensorData(rxFrame, out meterSerial, out pressure))
            {
                txtPressureSensorSerial2Overall.Text = Encoding.ASCII.GetString(meterSerial).TrimEnd('\0');
                txtPressure2Overall.Text = pressure.ToString();
            }
        }

        private bool TryGetTimezone(out string timezone)
        {
            timezone = string.Empty;

            if (cmbWriteTimezoneSetting.SelectedIndex == 0)
            {
                return true;
            }

            string text = cmbWriteTimezoneSetting.Text;

            if (!text.StartsWith("UTC") || text.Length != 9)
            {
                return false;
            }

            char sign = text[3];

            if (sign != '+' && sign != '-')
            {
                return false;
            }

            if (!int.TryParse(text.Substring(4, 2), out int hours) ||
                !int.TryParse(text.Substring(7, 2), out int minutes))
            {
                return false;
            }

            if (minutes != 0 && minutes != 30 && minutes != 45)
            {
                return false;
            }

            timezone = $"{(sign == '-' ? "-" : "")}{hours + minutes / 60.0f}";

            return true;
        }

        private string GetTimezoneText(float timezone)
        {
            int hours = (int)Math.Abs(timezone);
            int minutes = (int)Math.Round((Math.Abs(timezone) - hours) * 60);

            string sign = timezone < 0 ? "-" : "+";

            return $"UTC{sign}{hours:D2}:{minutes:D2}";
        }

        private void UpdatePulseMeter1Control()
        {
            bool enabled = chkPulseMeter1UseSetting.Checked;
            txtWritePulseMeter1SerialSetting.Enabled = enabled;
            txtWritePulseMeter1PulseFactorSetting.Enabled = enabled;
            txtWritePulseMeter1ForwardSetting.Enabled = enabled;
            txtWritePulseMeter1ReverseSetting.Enabled = enabled;
            cmbPulseMeter1TypeSetting.Enabled = enabled;

            switch (cmbPulseMeter1TypeSetting.SelectedIndex)
            {
                case 0:
                    cmbPulseMeter1Pin1Setting.Enabled = false;
                    cmbPulseMeter1Pin2Setting.Enabled = false;
                    cmbPulseMeter1EdgeSetting.Enabled = false;
                    break;

                case 1:
                    cmbPulseMeter1Pin1Setting.Enabled = true;
                    cmbPulseMeter1Pin2Setting.Enabled = false;
                    cmbPulseMeter1EdgeSetting.Enabled = true;
                    break;

                case 2:
                    cmbPulseMeter1Pin1Setting.Enabled = true;
                    cmbPulseMeter1Pin2Setting.Enabled = true;
                    cmbPulseMeter1EdgeSetting.Enabled = true;
                    break;

                default:
                    cmbPulseMeter1Pin1Setting.Enabled = false;
                    cmbPulseMeter1Pin2Setting.Enabled = false;
                    cmbPulseMeter1EdgeSetting.Enabled = false;
                    break;
            }
        }

        private void UpdatePulseMeter2Control()
        {
            bool enabled = chkPulseMeter2UseSetting.Checked;
            txtWritePulseMeter2SerialSetting.Enabled = enabled;
            txtWritePulseMeter2PulseFactorSetting.Enabled = enabled;
            txtWritePulseMeter2ForwardSetting.Enabled = enabled;
            txtWritePulseMeter2ReverseSetting.Enabled = enabled;
            cmbPulseMeter2TypeSetting.Enabled = enabled;

            switch (cmbPulseMeter2TypeSetting.SelectedIndex)
            {
                case 0:
                    cmbPulseMeter2Pin1Setting.Enabled = false;
                    cmbPulseMeter2Pin2Setting.Enabled = false;
                    cmbPulseMeter2EdgeSetting.Enabled = false;
                    break;

                case 1:
                    cmbPulseMeter2Pin1Setting.Enabled = true;
                    cmbPulseMeter2Pin2Setting.Enabled = false;
                    cmbPulseMeter2EdgeSetting.Enabled = true;
                    break;

                case 2:
                    cmbPulseMeter2Pin1Setting.Enabled = true;
                    cmbPulseMeter2Pin2Setting.Enabled = true;
                    cmbPulseMeter2EdgeSetting.Enabled = true;
                    break;

                default:
                    cmbPulseMeter2Pin1Setting.Enabled = false;
                    cmbPulseMeter2Pin2Setting.Enabled = false;
                    cmbPulseMeter2EdgeSetting.Enabled = false;
                    break;
            }
        }

        private void UpdatePulseMeter3Control()
        {
            bool enabled = chkPulseMeter3UseSetting.Checked;
            txtWritePulseMeter3SerialSetting.Enabled = enabled;
            txtWritePulseMeter3PulseFactorSetting.Enabled = enabled;
            txtWritePulseMeter3ForwardSetting.Enabled = enabled;
            txtWritePulseMeter3ReverseSetting.Enabled = enabled;
            cmbPulseMeter3TypeSetting.Enabled = enabled;

            switch (cmbPulseMeter3TypeSetting.SelectedIndex)
            {
                case 0:
                    cmbPulseMeter3Pin1Setting.Enabled = false;
                    cmbPulseMeter3Pin2Setting.Enabled = false;
                    cmbPulseMeter3EdgeSetting.Enabled = false;
                    break;

                case 1:
                    cmbPulseMeter3Pin1Setting.Enabled = true;
                    cmbPulseMeter3Pin2Setting.Enabled = false;
                    cmbPulseMeter3EdgeSetting.Enabled = true;
                    break;

                case 2:
                    cmbPulseMeter3Pin1Setting.Enabled = true;
                    cmbPulseMeter3Pin2Setting.Enabled = true;
                    cmbPulseMeter3EdgeSetting.Enabled = true;
                    break;

                default:
                    cmbPulseMeter3Pin1Setting.Enabled = false;
                    cmbPulseMeter3Pin2Setting.Enabled = false;
                    cmbPulseMeter3EdgeSetting.Enabled = false;
                    break;
            }
        }

        private void UpdatePulseMeter4Control()
        {
            bool enabled = chkPulseMeter4UseSetting.Checked;
            txtWritePulseMeter4SerialSetting.Enabled = enabled;
            txtWritePulseMeter4PulseFactorSetting.Enabled = enabled;
            txtWritePulseMeter4ForwardSetting.Enabled = enabled;
            txtWritePulseMeter4ReverseSetting.Enabled = enabled;
            cmbPulseMeter4TypeSetting.Enabled = enabled;

            switch (cmbPulseMeter4TypeSetting.SelectedIndex)
            {
                case 0:
                    cmbPulseMeter4Pin1Setting.Enabled = false;
                    cmbPulseMeter4Pin2Setting.Enabled = false;
                    cmbPulseMeter4EdgeSetting.Enabled = false;
                    break;

                case 1:
                    cmbPulseMeter4Pin1Setting.Enabled = true;
                    cmbPulseMeter4Pin2Setting.Enabled = false;
                    cmbPulseMeter4EdgeSetting.Enabled = true;
                    break;

                case 2:
                    cmbPulseMeter4Pin1Setting.Enabled = true;
                    cmbPulseMeter4Pin2Setting.Enabled = true;
                    cmbPulseMeter4EdgeSetting.Enabled = true;
                    break;

                default:
                    cmbPulseMeter4Pin1Setting.Enabled = false;
                    cmbPulseMeter4Pin2Setting.Enabled = false;
                    cmbPulseMeter4EdgeSetting.Enabled = false;
                    break;
            }
        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}