using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WM03A.Users.Model;
using WM03A.Users.ProtocolCommands;
using WM03A.Users.ProtocolParser;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static WM03A.Protocol;
using static WM03A.QueryParser;

namespace WM03A
{
    public partial class ucMain : UserControl
    {
        private ucLogin _ucLogin;

        private Protocol.AccessId _accessId;

        public event EventHandler LogoutRequested;

        private SerialPortManager _serialPortManager;

        private bool _isReading = false;

        private const int DEVICE_TIME_HEIGHT_PC = 60;
        private const int DEVICE_TIME_HEIGHT_MANUAL = 90;

        private bool _stopReadLatchData;
        private bool _stopReadEventData;

        private const int OTA_METADATA_A_OFFSET = 0x08008000 - 0x08008000;
        private const int OTA_METADATA_B_OFFSET = 0x08024000 - 0x08008000;

        private const int OTA_FIRMWARE_A_OFFSET = 0x08008800 - 0x08008000;
        private const int OTA_FIRMWARE_B_OFFSET = 0x08024800 - 0x08008000;

        private const int VERSION_SIZE = 10;

        private const int OTA_PACKET_SIZE = 1024;
        private const int OTA_METADATA_SIZE = 28;

        private const int PASSWORD_LENGTH = 8;

        private bool _stopFirmwareUpdate;

        public ucMain(SerialPortManager serialPortManager, Protocol.AccessId accessId, ucLogin ucLogin)
        {
            InitializeComponent();

            _accessId = accessId;

            ApplyAccessControl();

            _serialPortManager = serialPortManager;

            _ucLogin = ucLogin;
        }

        private void ucMain_Load(object sender, EventArgs e)
        {
            cmbWriteTimezoneSetting.SelectedIndex = 0;
            lblReconnectStatus.Text = string.Empty;
            rdoPcTimeSetting.Checked = true;
            lblTimeSettingStatus.Text = string.Empty;
            lblModuleSettingStatus.Text = string.Empty;
            lblPulseMeter1SettingStatus.Text = string.Empty;
            lblPulseMeter2SettingStatus.Text = string.Empty;
            lblPulseMeter3SettingStatus.Text = string.Empty;
            lblPulseMeter4SettingStatus.Text = string.Empty;
            lblModbusMeter1SettingStatus.Text = string.Empty;
            lblModbusMeter2SettingStatus.Text = string.Empty;
            lblModbusMeter3SettingStatus.Text = string.Empty;
            lblModbusMeter4SettingStatus.Text = string.Empty;
            lblPressureSensor1SettingStatus.Text = string.Empty;
            lblPressureSensor2SettingStatus.Text = string.Empty;
            lblLatchDateTime.Text = string.Empty;

            lblWriteSerialStatus.Text = string.Empty;
            lblWriteDeviceStatusAdvCfgStatus.Text = string.Empty;
            lblWriteMcuResetStatus.Text = string.Empty;

            lblChangePasswordStatus.Text = string.Empty;
            cmbRoleNewPassword.SelectedIndex = 0;

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

            cmbModbus1BaudSetting.SelectedIndex = 0;
            cmbModbus1FrameFormatSetting.SelectedIndex = 0;
            cmbModbus1FunctionCodeSetting.SelectedIndex = 0;
            cmbForward1DataTypeSetting.SelectedIndex = 0;
            cmbForward1WordSwapSetting.SelectedIndex = 0;
            cmbReverse1DataTypeSetting.SelectedIndex = 0;
            cmbReverse1WordSwapSetting.SelectedIndex= 0;
            cmbFlow1DataTypeSetting.SelectedIndex = 0;
            cmbFlow1WordSwapSetting.SelectedIndex = 0;

            cmbModbus2BaudSetting.SelectedIndex = 0;
            cmbModbus2FrameFormatSetting.SelectedIndex = 0;
            cmbModbus2FunctionCodeSetting.SelectedIndex = 0;
            cmbForward2DataTypeSetting.SelectedIndex = 0;
            cmbForward2WordSwapSetting.SelectedIndex = 0;
            cmbReverse2DataTypeSetting.SelectedIndex = 0;
            cmbReverse2WordSwapSetting.SelectedIndex = 0;
            cmbFlow2DataTypeSetting.SelectedIndex = 0;
            cmbFlow2WordSwapSetting.SelectedIndex = 0;

            cmbModbus3BaudSetting.SelectedIndex = 0;
            cmbModbus3FrameFormatSetting.SelectedIndex = 0;
            cmbModbus3FunctionCodeSetting.SelectedIndex = 0;
            cmbForward3DataTypeSetting.SelectedIndex = 0;
            cmbForward3WordSwapSetting.SelectedIndex = 0;
            cmbReverse3DataTypeSetting.SelectedIndex = 0;
            cmbReverse3WordSwapSetting.SelectedIndex = 0;
            cmbFlow3DataTypeSetting.SelectedIndex = 0;
            cmbFlow3WordSwapSetting.SelectedIndex = 0;

            cmbModbus4BaudSetting.SelectedIndex = 0;
            cmbModbus4FrameFormatSetting.SelectedIndex = 0;
            cmbModbus4FunctionCodeSetting.SelectedIndex = 0;
            cmbForward4DataTypeSetting.SelectedIndex = 0;
            cmbForward4WordSwapSetting.SelectedIndex = 0;
            cmbReverse4DataTypeSetting.SelectedIndex = 0;
            cmbReverse4WordSwapSetting.SelectedIndex = 0;
            cmbFlow4DataTypeSetting.SelectedIndex = 0;
            cmbFlow4WordSwapSetting.SelectedIndex = 0;

            UpdatePulseMeter1Control();
            UpdatePulseMeter2Control();
            UpdatePulseMeter3Control();
            UpdatePulseMeter4Control();

            UpdateModbusMeter1Control();
            UpdateModbusMeter2Control();
            UpdateModbusMeter3Control();
            UpdateModbusMeter4Control();

            UpdatePressureSensor1Control();
            UpdatePressureSensor2Control();
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

        private async void btnReconnect_Click(object sender, EventArgs e)
        {
            btnReconnect.Enabled = false;

            try
            {
                bool success = await _ucLogin.ReconnectAsync();

                if (success)
                {
                    lblReconnectStatus.Text = "Kết nối lại thành công";
                    await Task.Delay(1000);
                    lblReconnectStatus.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Kết nối lại thất bại.", "Reconnect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                btnReconnect.Enabled = true;
            }
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
            byte[] payload = new byte[PASSWORD_LENGTH];
            Pack(
                encrypt: true,
                serial: PROTOCOL_MODULE_SERIAL_COMMON,
                cmd: (byte)CmdCode.Access,
                id: (byte)AccessId.Logout,
                payload: payload,
                out byte[] txFrame);

            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.IpEndpoint(rxFrame, out string ip, out string port))
            {
                txtReadIpSetting.Text = ip;
                txtReadPortSetting.Text = port;
            }

            // Read Latch Period
            GetCommands.LatchPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok)
            {
                ushort latchPeriod = GetParser.LatchPeriod(rxFrame);
                txtReadLatchSetting.Text = latchPeriod.ToString();
            }

            // Read Push Period
            GetCommands.PushPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok)
            {
                ushort PushPeriod = GetParser.PushPeriod(rxFrame);
                txtReadPushSetting.Text = PushPeriod.ToString();
            }

            // Read Timezone
            GetCommands.Timezone(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
                if (!ok || !SetParser.IpEndpoint(rxFrame))
                {
                    writeStatus = false;
                }
            }

            if (SetCommands.ModuleConfig(txtWriteLatchSetting.Text, txtWritePushSetting.Text, timezone, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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

        private void chkPulseMeter1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter1Control();
        }
        private void cmbPulseMeter1TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter1Control();
        }
        private void btnPulseMeter1CopySetting_Click(object sender, EventArgs e)
        {
            chkPulseMeter1UseSetting.Checked = txtReadPulseMeter1UseSetting.Text == "Đang sử dụng";

            txtWritePulseMeter1SerialSetting.Text = txtReadPulseMeter1SerialSetting.Text;
            txtWritePulseMeter1PulseFactorSetting.Text = txtReadPulseMeter1PulseFactorSetting.Text;
            cmbPulseMeter1TypeSetting.Text = txtReadPulseMeter1TypeSetting.Text;
            cmbPulseMeter1Pin1Setting.Text = txtReadPulseMeter1Pin1Setting.Text;
            cmbPulseMeter1Pin2Setting.Text = txtReadPulseMeter1Pin2Setting.Text;
            cmbPulseMeter1EdgeSetting.Text = txtReadPulseMeter1EdgeSetting.Text;
            txtWritePulseMeter1ForwardSetting.Text = txtReadPulseMeter1ForwardSetting.Text;
            txtWritePulseMeter1ReverseSetting.Text = txtReadPulseMeter1ReverseSetting.Text;

            UpdatePulseMeter1Control();
        }
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter1ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter1ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
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
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

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


        private void chkPulseMeter2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter2Control();
        }

        private void cmbPulseMeter2TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter2Control();
        }
        private void btnPulseMeter2CopySetting_Click(object sender, EventArgs e)
        {
            chkPulseMeter2UseSetting.Checked = txtReadPulseMeter2UseSetting.Text == "Đang sử dụng";

            txtWritePulseMeter2SerialSetting.Text = txtReadPulseMeter2SerialSetting.Text;
            txtWritePulseMeter2PulseFactorSetting.Text = txtReadPulseMeter2PulseFactorSetting.Text;
            cmbPulseMeter2TypeSetting.Text = txtReadPulseMeter2TypeSetting.Text;
            cmbPulseMeter2Pin1Setting.Text = txtReadPulseMeter2Pin1Setting.Text;
            cmbPulseMeter2Pin2Setting.Text = txtReadPulseMeter2Pin2Setting.Text;
            cmbPulseMeter2EdgeSetting.Text = txtReadPulseMeter2EdgeSetting.Text;
            txtWritePulseMeter2ForwardSetting.Text = txtReadPulseMeter2ForwardSetting.Text;
            txtWritePulseMeter2ReverseSetting.Text = txtReadPulseMeter2ReverseSetting.Text;

            UpdatePulseMeter2Control();
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter2ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter2ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
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
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

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


        private void chkPulseMeter3UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter3Control();
        }
        private void cmbPulseMeter3TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter3Control();
        }
        private void btnPulseMeter3CopySetting_Click(object sender, EventArgs e)
        {
            chkPulseMeter3UseSetting.Checked = txtReadPulseMeter3UseSetting.Text == "Đang sử dụng";

            txtWritePulseMeter3SerialSetting.Text = txtReadPulseMeter3SerialSetting.Text;
            txtWritePulseMeter3PulseFactorSetting.Text = txtReadPulseMeter3PulseFactorSetting.Text;
            cmbPulseMeter3TypeSetting.Text = txtReadPulseMeter3TypeSetting.Text;
            cmbPulseMeter3Pin1Setting.Text = txtReadPulseMeter3Pin1Setting.Text;
            cmbPulseMeter3Pin2Setting.Text = txtReadPulseMeter3Pin2Setting.Text;
            cmbPulseMeter3EdgeSetting.Text = txtReadPulseMeter3EdgeSetting.Text;
            txtWritePulseMeter3ForwardSetting.Text = txtReadPulseMeter3ForwardSetting.Text;
            txtWritePulseMeter3ReverseSetting.Text = txtReadPulseMeter3ReverseSetting.Text;

            UpdatePulseMeter3Control();
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter3ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter3ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
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
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

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


        private void chkPulseMeter4UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter4Control();
        }

        private void cmbPulseMeter4TypeSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePulseMeter4Control();
        }
        private void btnPulseMeter4CopySetting_Click(object sender, EventArgs e)
        {
            chkPulseMeter4UseSetting.Checked = txtReadPulseMeter4UseSetting.Text == "Đang sử dụng";

            txtWritePulseMeter4SerialSetting.Text = txtReadPulseMeter4SerialSetting.Text;
            txtWritePulseMeter4PulseFactorSetting.Text = txtReadPulseMeter4PulseFactorSetting.Text;
            cmbPulseMeter4TypeSetting.Text = txtReadPulseMeter4TypeSetting.Text;
            cmbPulseMeter4Pin1Setting.Text = txtReadPulseMeter4Pin1Setting.Text;
            cmbPulseMeter4Pin2Setting.Text = txtReadPulseMeter4Pin2Setting.Text;
            cmbPulseMeter4EdgeSetting.Text = txtReadPulseMeter4EdgeSetting.Text;
            txtWritePulseMeter4ForwardSetting.Text = txtReadPulseMeter4ForwardSetting.Text;
            txtWritePulseMeter4ReverseSetting.Text = txtReadPulseMeter4ReverseSetting.Text;

            UpdatePulseMeter4Control();
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out QueryParser.MeterData meterData))
            {
                txtReadPulseMeter4ForwardSetting.Text = meterData.ForwardTotalizer.ToString();
                txtReadPulseMeter4ReverseSetting.Text = meterData.ReverseTotalizer.ToString();
            }
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
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

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


        //-----------------------Modbus Meter Setting--------------------------------//

        private void chkModbus1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter1Control();
        }

        private void chkForward1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter1Control();
        }

        private void chkReverse1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter1Control();
        }

        private void chkFlow1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter1Control();
        }

        private void btnModbusMeter1CopySetting_Click(object sender, EventArgs e)
        {
            chkModbus1UseSetting.Checked = txtReadModbus1UseSetting.Text == "Đang sử dụng";

            txtWriteModbus1SerialSetting.Text = txtReadModbus1SerialSetting.Text;
            txtWriteModbus1SlaveAddrSetting.Text = txtReadModbus1SlaveAddrSetting.Text;

            cmbModbus1BaudSetting.Text = txtReadModbus1BaudSetting.Text;
            cmbModbus1FrameFormatSetting.Text = txtReadModbus1FrameFormatSetting.Text;
            cmbModbus1FunctionCodeSetting.Text = txtReadModbus1FunctionCodeSetting.Text;

            chkForward1UseSetting.Checked = txtReadForward1UseSetting.Text == "Đang sử dụng";
            txtWriteForward1RegAddrSetting.Text = txtReadForward1RegAddrSetting.Text;
            cmbForward1DataTypeSetting.Text = txtReadForward1DataTypeSetting.Text;
            cmbForward1WordSwapSetting.Text = txtReadForward1WordSwapSetting.Text;
            txtWriteForward1MultiplierSetting.Text = txtReadForward1MultiplierSetting.Text;

            chkReverse1UseSetting.Checked = txtReadReverse1UseSetting.Text == "Đang sử dụng";
            txtWriteReverse1RegAddrSetting.Text = txtReadReverse1RegAddrSetting.Text;
            cmbReverse1DataTypeSetting.Text = txtReadReverse1DataTypeSetting.Text;
            cmbReverse1WordSwapSetting.Text = txtReadReverse1WordSwapSetting.Text;
            txtWriteReverse1MultiplierSetting.Text = txtReadReverse1MultiplierSetting.Text;

            chkFlow1UseSetting.Checked = txtReadFlow1UseSetting.Text == "Đang sử dụng";
            txtWriteFlow1RegAddrSetting.Text = txtReadFlow1RegAddrSetting.Text;
            cmbFlow1DataTypeSetting.Text = txtReadFlow1DataTypeSetting.Text;
            cmbFlow1WordSwapSetting.Text = txtReadFlow1WordSwapSetting.Text;
            txtWriteFlow1MultiplierSetting.Text = txtReadFlow1MultiplierSetting.Text;

            UpdateModbusMeter1Control();
        }

        private async void btnReadModbusMeter1Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
                (byte)ConfigModbusMeterId.MeterEnable,
                (byte)ConfigModbusMeterId.SerialNumber,
                (byte)ConfigModbusMeterId.SlaveAddress,
                (byte)ConfigModbusMeterId.Baudrate,
                (byte)ConfigModbusMeterId.SerialConfig,
                (byte)ConfigModbusMeterId.ReadFuncCode,
                (byte)ConfigModbusMeterId.ForwardTotalEnable,
                (byte)ConfigModbusMeterId.ForwardTotalRegAddr,
                (byte)ConfigModbusMeterId.ForwardTotalDataType,
                (byte)ConfigModbusMeterId.ForwardTotalWordSwap,
                (byte)ConfigModbusMeterId.ForwardTotalMultiplier,
                (byte)ConfigModbusMeterId.ReverseTotalEnable,
                (byte)ConfigModbusMeterId.ReverseTotalRegAddr,
                (byte)ConfigModbusMeterId.ReverseTotalDataType,
                (byte)ConfigModbusMeterId.ReverseTotalWordSwap,
                (byte)ConfigModbusMeterId.ReverseTotalMultiplier,
                (byte)ConfigModbusMeterId.FlowRateEnable,
                (byte)ConfigModbusMeterId.FlowRateRegAddr,
                (byte)ConfigModbusMeterId.FlowRateDataType,
                (byte)ConfigModbusMeterId.FlowRateWordSwap,
                (byte)ConfigModbusMeterId.FlowRateMultiplier
            };

            txtReadModbus1UseSetting.Clear();
            txtReadModbus1SerialSetting.Clear();
            txtReadModbus1SlaveAddrSetting.Clear();
            txtReadModbus1BaudSetting.Clear();
            txtReadModbus1FrameFormatSetting.Clear();
            txtReadModbus1FunctionCodeSetting.Clear();
            txtReadForward1UseSetting.Clear();
            txtReadForward1RegAddrSetting.Clear();
            txtReadForward1DataTypeSetting.Clear();
            txtReadForward1WordSwapSetting.Clear();
            txtReadForward1MultiplierSetting.Clear();
            txtReadReverse1UseSetting.Clear();
            txtReadReverse1RegAddrSetting.Clear();
            txtReadReverse1DataTypeSetting.Clear();
            txtReadReverse1WordSwapSetting.Clear();
            txtReadReverse1MultiplierSetting.Clear();
            txtReadFlow1UseSetting.Clear();
            txtReadFlow1RegAddrSetting.Clear();
            txtReadFlow1DataTypeSetting.Clear();
            txtReadFlow1WordSwapSetting.Clear();
            txtReadFlow1MultiplierSetting.Clear();

            GetCommands.ModbusMeter(0, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.ModbusMeter(rxFrame, out ModbusMeterConfig modbusMeterConfig))
            {
                if (!modbusMeterConfig.MeterEnable)
                {
                    txtReadModbus1UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadModbus1UseSetting.Text = "Đang sử dụng";
                    txtReadModbus1SerialSetting.Text = modbusMeterConfig.SerialNumber;
                    txtReadModbus1SlaveAddrSetting.Text = modbusMeterConfig.SlaveAddress.ToString();
                    txtReadModbus1BaudSetting.Text = modbusMeterConfig.BaudRate.ToString();
                    switch (modbusMeterConfig.SerialConfig)
                    {
                        case ((byte)ModbusSerialConfig.Modbus_8N1):
                            txtReadModbus1FrameFormatSetting.Text = "8N1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8O1):
                            txtReadModbus1FrameFormatSetting.Text = "8O1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8E1):
                            txtReadModbus1FrameFormatSetting.Text = "8E1";
                            break;

                        default:
                            break;
                    }
                    txtReadModbus1FunctionCodeSetting.Text = $"0x{modbusMeterConfig.ReadFuncCode:X2}";

                    if (!modbusMeterConfig.ForwardTotal.ParameterEnable)
                    {
                        txtReadForward1UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadForward1UseSetting.Text = "Đang sử dụng";
                        txtReadForward1RegAddrSetting.Text = modbusMeterConfig.ForwardTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ForwardTotal.DataType != 0)
                        {
                            txtReadForward1DataTypeSetting.Text = cmbForward1DataTypeSetting.Items[modbusMeterConfig.ForwardTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ForwardTotal.WordSwap)
                        {
                            txtReadForward1WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadForward1WordSwapSetting.Text = "Không";
                        }
                        txtReadForward1MultiplierSetting.Text = modbusMeterConfig.ForwardTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.ReverseTotal.ParameterEnable)
                    {
                        txtReadReverse1UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadReverse1UseSetting.Text = "Đang sử dụng";
                        txtReadReverse1RegAddrSetting.Text = modbusMeterConfig.ReverseTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ReverseTotal.DataType != 0)
                        {
                            txtReadReverse1DataTypeSetting.Text = cmbReverse1DataTypeSetting.Items[modbusMeterConfig.ReverseTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ReverseTotal.WordSwap)
                        {
                            txtReadReverse1WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadReverse1WordSwapSetting.Text = "Không";
                        }
                        txtReadReverse1MultiplierSetting.Text = modbusMeterConfig.ReverseTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.FlowRate.ParameterEnable)
                    {
                        txtReadFlow1UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadFlow1UseSetting.Text = "Đang sử dụng";
                        txtReadFlow1RegAddrSetting.Text = modbusMeterConfig.FlowRate.RegisterAddress.ToString();
                        if (modbusMeterConfig.FlowRate.DataType != 0)
                        {
                            txtReadFlow1DataTypeSetting.Text = cmbFlow1DataTypeSetting.Items[modbusMeterConfig.FlowRate.DataType].ToString();
                        }
                        if (modbusMeterConfig.FlowRate.WordSwap)
                        {
                            txtReadFlow1WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadFlow1WordSwapSetting.Text = "Không";
                        }
                        txtReadFlow1MultiplierSetting.Text = modbusMeterConfig.FlowRate.Multiplier.ToString();
                    }

                }
            }
        }

        private async void btnWriteModbusMeter1Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkModbus1UseSetting.Checked;

            string serialNumber = txtWriteModbus1SerialSetting.Text.Trim();
            byte? slaveAddress = null;
            uint? baudRate = null;
            byte? serialConfig = null;
            byte? readFuncCode = null;

            bool? forwardEnable = null;
            ushort? forwardRegAddr = null;
            byte? forwardDataType = null;
            bool? forwardWordSwap = null;
            sbyte? forwardMultiplier = null;

            bool? reverseEnable = null;
            ushort? reverseRegAddr = null;
            byte? reverseDataType = null;
            bool? reverseWordSwap = null;
            sbyte? reverseMultiplier = null;

            bool? flowEnable = null;
            ushort? flowRegAddr = null;
            byte? flowDataType = null;
            bool? flowWordSwap = null;
            sbyte? flowMultiplier = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblModbusMeter1SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!byte.TryParse(txtWriteModbus1SlaveAddrSetting.Text, out byte slaveAddressValue))
                {
                    lblModbusMeter1SettingStatus.Text = "Slave Address không hợp lệ.";
                    return;
                }

                if (!uint.TryParse(cmbModbus1BaudSetting.Text, out uint baudRateValue))
                {
                    lblModbusMeter1SettingStatus.Text = "Baud Rate không hợp lệ.";
                    return;
                }

                if (cmbModbus1FrameFormatSetting.SelectedIndex == 0)
                {
                    lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Frame Format.";
                    return;
                }

                if (cmbModbus1FunctionCodeSetting.SelectedIndex == 0)
                {
                    lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }

                slaveAddress = slaveAddressValue;
                baudRate = baudRateValue;
                switch (cmbModbus1FrameFormatSetting.Text)
                {
                    case "8N1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8N1;
                        break;

                    case "8O1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8O1;
                        break;

                    case "8E1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8E1;
                        break;

                    default:
                        lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Frame Format.";
                        return;
                }
                if (!byte.TryParse(cmbModbus1FunctionCodeSetting.Text.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out byte functionCode))
                {
                    lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }
                readFuncCode = functionCode;

                forwardEnable = chkForward1UseSetting.Checked;
                reverseEnable = chkReverse1UseSetting.Checked;
                flowEnable = chkFlow1UseSetting.Checked;

                if (forwardEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteForward1RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter1SettingStatus.Text = "Forward Register Address không hợp lệ.";
                        return;
                    }

                    forwardRegAddr = value;

                    if (cmbForward1DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Forward Data Type.";
                        return;
                    }

                    forwardDataType = (byte)cmbForward1DataTypeSetting.SelectedIndex;
                    forwardWordSwap = cmbForward1WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteForward1MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter1SettingStatus.Text = "Forward Multiplier không hợp lệ.";
                        return;
                    }

                    forwardMultiplier = multiplier;
                }

                if (reverseEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteReverse1RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter1SettingStatus.Text = "Reverse Register Address không hợp lệ.";
                        return;
                    }

                    reverseRegAddr = value;

                    if (cmbReverse1DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Reverse Data Type.";
                        return;
                    }

                    reverseDataType = (byte)cmbReverse1DataTypeSetting.SelectedIndex;
                    reverseWordSwap = cmbReverse1WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteReverse1MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter1SettingStatus.Text = "Reverse Multiplier không hợp lệ.";
                        return;
                    }

                    reverseMultiplier = multiplier;
                }

                if (flowEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteFlow1RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter1SettingStatus.Text = "Flow Rate Register Address không hợp lệ.";
                        return;
                    }

                    flowRegAddr = value;

                    if (cmbFlow1DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter1SettingStatus.Text = "Vui lòng chọn Flow Rate Data Type.";
                        return;
                    }

                    flowDataType = (byte)cmbFlow1DataTypeSetting.SelectedIndex;
                    flowWordSwap = cmbFlow1WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteFlow1MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter1SettingStatus.Text = "Flow Rate Multiplier không hợp lệ.";
                        return;
                    }

                    flowMultiplier = multiplier;
                }
            }

            if (SetCommands.ModbusMeter(
                0,
                enabled,
                serialNumber,
                slaveAddress,
                baudRate,
                serialConfig,
                readFuncCode,
                forwardEnable,
                forwardRegAddr,
                forwardDataType,
                forwardWordSwap,
                forwardMultiplier,
                reverseEnable,
                reverseRegAddr,
                reverseDataType,
                reverseWordSwap,
                reverseMultiplier,
                flowEnable,
                flowRegAddr,
                flowDataType,
                flowWordSwap,
                flowMultiplier,
                out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                if (ok && SetParser.ModbusMeter(rxFrame))
                {
                    lblModbusMeter1SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblModbusMeter1SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblModbusMeter1SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblModbusMeter1SettingStatus.Text = string.Empty;
        }

        private void chkModbus2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter2Control();
        }

        private void chkForward2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter2Control();
        }

        private void chkReverse2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter2Control();
        }

        private void chkFlow2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter2Control();
        }

        private void btnModbusMeter2CopySetting_Click(object sender, EventArgs e)
        {
            chkModbus2UseSetting.Checked = txtReadModbus2UseSetting.Text == "Đang sử dụng";

            txtWriteModbus2SerialSetting.Text = txtReadModbus2SerialSetting.Text;
            txtWriteModbus2SlaveAddrSetting.Text = txtReadModbus2SlaveAddrSetting.Text;

            cmbModbus2BaudSetting.Text = txtReadModbus2BaudSetting.Text;
            cmbModbus2FrameFormatSetting.Text = txtReadModbus2FrameFormatSetting.Text;
            cmbModbus2FunctionCodeSetting.Text = txtReadModbus2FunctionCodeSetting.Text;

            chkForward2UseSetting.Checked = txtReadForward2UseSetting.Text == "Đang sử dụng";
            txtWriteForward2RegAddrSetting.Text = txtReadForward2RegAddrSetting.Text;
            cmbForward2DataTypeSetting.Text = txtReadForward2DataTypeSetting.Text;
            cmbForward2WordSwapSetting.Text = txtReadForward2WordSwapSetting.Text;
            txtWriteForward2MultiplierSetting.Text = txtReadForward2MultiplierSetting.Text;

            chkReverse2UseSetting.Checked = txtReadReverse2UseSetting.Text == "Đang sử dụng";
            txtWriteReverse2RegAddrSetting.Text = txtReadReverse2RegAddrSetting.Text;
            cmbReverse2DataTypeSetting.Text = txtReadReverse2DataTypeSetting.Text;
            cmbReverse2WordSwapSetting.Text = txtReadReverse2WordSwapSetting.Text;
            txtWriteReverse2MultiplierSetting.Text = txtReadReverse2MultiplierSetting.Text;

            chkFlow2UseSetting.Checked = txtReadFlow2UseSetting.Text == "Đang sử dụng";
            txtWriteFlow2RegAddrSetting.Text = txtReadFlow2RegAddrSetting.Text;
            cmbFlow2DataTypeSetting.Text = txtReadFlow2DataTypeSetting.Text;
            cmbFlow2WordSwapSetting.Text = txtReadFlow2WordSwapSetting.Text;
            txtWriteFlow2MultiplierSetting.Text = txtReadFlow2MultiplierSetting.Text;

            UpdateModbusMeter2Control();
        }

        private async void btnReadModbusMeter2Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
        (byte)ConfigModbusMeterId.MeterEnable,
        (byte)ConfigModbusMeterId.SerialNumber,
        (byte)ConfigModbusMeterId.SlaveAddress,
        (byte)ConfigModbusMeterId.Baudrate,
        (byte)ConfigModbusMeterId.SerialConfig,
        (byte)ConfigModbusMeterId.ReadFuncCode,
        (byte)ConfigModbusMeterId.ForwardTotalEnable,
        (byte)ConfigModbusMeterId.ForwardTotalRegAddr,
        (byte)ConfigModbusMeterId.ForwardTotalDataType,
        (byte)ConfigModbusMeterId.ForwardTotalWordSwap,
        (byte)ConfigModbusMeterId.ForwardTotalMultiplier,
        (byte)ConfigModbusMeterId.ReverseTotalEnable,
        (byte)ConfigModbusMeterId.ReverseTotalRegAddr,
        (byte)ConfigModbusMeterId.ReverseTotalDataType,
        (byte)ConfigModbusMeterId.ReverseTotalWordSwap,
        (byte)ConfigModbusMeterId.ReverseTotalMultiplier,
        (byte)ConfigModbusMeterId.FlowRateEnable,
        (byte)ConfigModbusMeterId.FlowRateRegAddr,
        (byte)ConfigModbusMeterId.FlowRateDataType,
        (byte)ConfigModbusMeterId.FlowRateWordSwap,
        (byte)ConfigModbusMeterId.FlowRateMultiplier
    };

            txtReadModbus2UseSetting.Clear();
            txtReadModbus2SerialSetting.Clear();
            txtReadModbus2SlaveAddrSetting.Clear();
            txtReadModbus2BaudSetting.Clear();
            txtReadModbus2FrameFormatSetting.Clear();
            txtReadModbus2FunctionCodeSetting.Clear();
            txtReadForward2UseSetting.Clear();
            txtReadForward2RegAddrSetting.Clear();
            txtReadForward2DataTypeSetting.Clear();
            txtReadForward2WordSwapSetting.Clear();
            txtReadForward2MultiplierSetting.Clear();
            txtReadReverse2UseSetting.Clear();
            txtReadReverse2RegAddrSetting.Clear();
            txtReadReverse2DataTypeSetting.Clear();
            txtReadReverse2WordSwapSetting.Clear();
            txtReadReverse2MultiplierSetting.Clear();
            txtReadFlow2UseSetting.Clear();
            txtReadFlow2RegAddrSetting.Clear();
            txtReadFlow2DataTypeSetting.Clear();
            txtReadFlow2WordSwapSetting.Clear();
            txtReadFlow2MultiplierSetting.Clear();

            GetCommands.ModbusMeter(1, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.ModbusMeter(rxFrame, out ModbusMeterConfig modbusMeterConfig))
            {
                if (!modbusMeterConfig.MeterEnable)
                {
                    txtReadModbus2UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadModbus2UseSetting.Text = "Đang sử dụng";
                    txtReadModbus2SerialSetting.Text = modbusMeterConfig.SerialNumber;
                    txtReadModbus2SlaveAddrSetting.Text = modbusMeterConfig.SlaveAddress.ToString();
                    txtReadModbus2BaudSetting.Text = modbusMeterConfig.BaudRate.ToString();
                    switch (modbusMeterConfig.SerialConfig)
                    {
                        case ((byte)ModbusSerialConfig.Modbus_8N1):
                            txtReadModbus2FrameFormatSetting.Text = "8N1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8O1):
                            txtReadModbus2FrameFormatSetting.Text = "8O1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8E1):
                            txtReadModbus2FrameFormatSetting.Text = "8E1";
                            break;

                        default:
                            break;
                    }
                    txtReadModbus2FunctionCodeSetting.Text = $"0x{modbusMeterConfig.ReadFuncCode:X2}";

                    if (!modbusMeterConfig.ForwardTotal.ParameterEnable)
                    {
                        txtReadForward2UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadForward2UseSetting.Text = "Đang sử dụng";
                        txtReadForward2RegAddrSetting.Text = modbusMeterConfig.ForwardTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ForwardTotal.DataType != 0)
                        {
                            txtReadForward2DataTypeSetting.Text = cmbForward2DataTypeSetting.Items[modbusMeterConfig.ForwardTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ForwardTotal.WordSwap)
                        {
                            txtReadForward2WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadForward2WordSwapSetting.Text = "Không";
                        }
                        txtReadForward2MultiplierSetting.Text = modbusMeterConfig.ForwardTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.ReverseTotal.ParameterEnable)
                    {
                        txtReadReverse2UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadReverse2UseSetting.Text = "Đang sử dụng";
                        txtReadReverse2RegAddrSetting.Text = modbusMeterConfig.ReverseTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ReverseTotal.DataType != 0)
                        {
                            txtReadReverse2DataTypeSetting.Text = cmbReverse2DataTypeSetting.Items[modbusMeterConfig.ReverseTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ReverseTotal.WordSwap)
                        {
                            txtReadReverse2WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadReverse2WordSwapSetting.Text = "Không";
                        }
                        txtReadReverse2MultiplierSetting.Text = modbusMeterConfig.ReverseTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.FlowRate.ParameterEnable)
                    {
                        txtReadFlow2UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadFlow2UseSetting.Text = "Đang sử dụng";
                        txtReadFlow2RegAddrSetting.Text = modbusMeterConfig.FlowRate.RegisterAddress.ToString();
                        if (modbusMeterConfig.FlowRate.DataType != 0)
                        {
                            txtReadFlow2DataTypeSetting.Text = cmbFlow2DataTypeSetting.Items[modbusMeterConfig.FlowRate.DataType].ToString();
                        }
                        if (modbusMeterConfig.FlowRate.WordSwap)
                        {
                            txtReadFlow2WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadFlow2WordSwapSetting.Text = "Không";
                        }
                        txtReadFlow2MultiplierSetting.Text = modbusMeterConfig.FlowRate.Multiplier.ToString();
                    }
                }
            }
        }

        private async void btnWriteModbusMeter2Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkModbus2UseSetting.Checked;

            string serialNumber = txtWriteModbus2SerialSetting.Text.Trim();
            byte? slaveAddress = null;
            uint? baudRate = null;
            byte? serialConfig = null;
            byte? readFuncCode = null;

            bool? forwardEnable = null;
            ushort? forwardRegAddr = null;
            byte? forwardDataType = null;
            bool? forwardWordSwap = null;
            sbyte? forwardMultiplier = null;

            bool? reverseEnable = null;
            ushort? reverseRegAddr = null;
            byte? reverseDataType = null;
            bool? reverseWordSwap = null;
            sbyte? reverseMultiplier = null;

            bool? flowEnable = null;
            ushort? flowRegAddr = null;
            byte? flowDataType = null;
            bool? flowWordSwap = null;
            sbyte? flowMultiplier = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblModbusMeter2SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!byte.TryParse(txtWriteModbus2SlaveAddrSetting.Text, out byte slaveAddressValue))
                {
                    lblModbusMeter2SettingStatus.Text = "Slave Address không hợp lệ.";
                    return;
                }

                if (!uint.TryParse(cmbModbus2BaudSetting.Text, out uint baudRateValue))
                {
                    lblModbusMeter2SettingStatus.Text = "Baud Rate không hợp lệ.";
                    return;
                }

                if (cmbModbus2FrameFormatSetting.SelectedIndex == 0)
                {
                    lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Frame Format.";
                    return;
                }

                if (cmbModbus2FunctionCodeSetting.SelectedIndex == 0)
                {
                    lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }

                slaveAddress = slaveAddressValue;
                baudRate = baudRateValue;
                switch (cmbModbus2FrameFormatSetting.Text)
                {
                    case "8N1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8N1;
                        break;

                    case "8O1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8O1;
                        break;

                    case "8E1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8E1;
                        break;

                    default:
                        lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Frame Format.";
                        return;
                }
                if (!byte.TryParse(cmbModbus2FunctionCodeSetting.Text.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out byte functionCode))
                {
                    lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }
                readFuncCode = functionCode;

                forwardEnable = chkForward2UseSetting.Checked;
                reverseEnable = chkReverse2UseSetting.Checked;
                flowEnable = chkFlow2UseSetting.Checked;

                if (forwardEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteForward2RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter2SettingStatus.Text = "Forward Register Address không hợp lệ.";
                        return;
                    }

                    forwardRegAddr = value;

                    if (cmbForward2DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Forward Data Type.";
                        return;
                    }

                    forwardDataType = (byte)cmbForward2DataTypeSetting.SelectedIndex;
                    forwardWordSwap = cmbForward2WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteForward2MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter2SettingStatus.Text = "Forward Multiplier không hợp lệ.";
                        return;
                    }

                    forwardMultiplier = multiplier;
                }

                if (reverseEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteReverse2RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter2SettingStatus.Text = "Reverse Register Address không hợp lệ.";
                        return;
                    }

                    reverseRegAddr = value;

                    if (cmbReverse2DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Reverse Data Type.";
                        return;
                    }

                    reverseDataType = (byte)cmbReverse2DataTypeSetting.SelectedIndex;
                    reverseWordSwap = cmbReverse2WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteReverse2MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter2SettingStatus.Text = "Reverse Multiplier không hợp lệ.";
                        return;
                    }

                    reverseMultiplier = multiplier;
                }

                if (flowEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteFlow2RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter2SettingStatus.Text = "Flow Rate Register Address không hợp lệ.";
                        return;
                    }

                    flowRegAddr = value;

                    if (cmbFlow2DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter2SettingStatus.Text = "Vui lòng chọn Flow Rate Data Type.";
                        return;
                    }

                    flowDataType = (byte)cmbFlow2DataTypeSetting.SelectedIndex;
                    flowWordSwap = cmbFlow2WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteFlow2MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter2SettingStatus.Text = "Flow Rate Multiplier không hợp lệ.";
                        return;
                    }

                    flowMultiplier = multiplier;
                }
            }

            if (SetCommands.ModbusMeter(
                1,
                enabled,
                serialNumber,
                slaveAddress,
                baudRate,
                serialConfig,
                readFuncCode,
                forwardEnable,
                forwardRegAddr,
                forwardDataType,
                forwardWordSwap,
                forwardMultiplier,
                reverseEnable,
                reverseRegAddr,
                reverseDataType,
                reverseWordSwap,
                reverseMultiplier,
                flowEnable,
                flowRegAddr,
                flowDataType,
                flowWordSwap,
                flowMultiplier,
                out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                if (ok && SetParser.ModbusMeter(rxFrame))
                {
                    lblModbusMeter2SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblModbusMeter2SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblModbusMeter2SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblModbusMeter2SettingStatus.Text = string.Empty;
        }

        private void chkModbus3UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter3Control();
        }

        private void chkForward3UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter3Control();
        }

        private void chkReverse3UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter3Control();
        }

        private void chkFlow3UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter3Control();
        }

        private void btnModbusMeter3CopySetting_Click(object sender, EventArgs e)
        {
            chkModbus3UseSetting.Checked = txtReadModbus3UseSetting.Text == "Đang sử dụng";

            txtWriteModbus3SerialSetting.Text = txtReadModbus3SerialSetting.Text;
            txtWriteModbus3SlaveAddrSetting.Text = txtReadModbus3SlaveAddrSetting.Text;

            cmbModbus3BaudSetting.Text = txtReadModbus3BaudSetting.Text;
            cmbModbus3FrameFormatSetting.Text = txtReadModbus3FrameFormatSetting.Text;
            cmbModbus3FunctionCodeSetting.Text = txtReadModbus3FunctionCodeSetting.Text;

            chkForward3UseSetting.Checked = txtReadForward3UseSetting.Text == "Đang sử dụng";
            txtWriteForward3RegAddrSetting.Text = txtReadForward3RegAddrSetting.Text;
            cmbForward3DataTypeSetting.Text = txtReadForward3DataTypeSetting.Text;
            cmbForward3WordSwapSetting.Text = txtReadForward3WordSwapSetting.Text;
            txtWriteForward3MultiplierSetting.Text = txtReadForward3MultiplierSetting.Text;

            chkReverse3UseSetting.Checked = txtReadReverse3UseSetting.Text == "Đang sử dụng";
            txtWriteReverse3RegAddrSetting.Text = txtReadReverse3RegAddrSetting.Text;
            cmbReverse3DataTypeSetting.Text = txtReadReverse3DataTypeSetting.Text;
            cmbReverse3WordSwapSetting.Text = txtReadReverse3WordSwapSetting.Text;
            txtWriteReverse3MultiplierSetting.Text = txtReadReverse3MultiplierSetting.Text;

            chkFlow3UseSetting.Checked = txtReadFlow3UseSetting.Text == "Đang sử dụng";
            txtWriteFlow3RegAddrSetting.Text = txtReadFlow3RegAddrSetting.Text;
            cmbFlow3DataTypeSetting.Text = txtReadFlow3DataTypeSetting.Text;
            cmbFlow3WordSwapSetting.Text = txtReadFlow3WordSwapSetting.Text;
            txtWriteFlow3MultiplierSetting.Text = txtReadFlow3MultiplierSetting.Text;

            UpdateModbusMeter3Control();
        }

        private async void btnReadModbusMeter3Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
        (byte)ConfigModbusMeterId.MeterEnable,
        (byte)ConfigModbusMeterId.SerialNumber,
        (byte)ConfigModbusMeterId.SlaveAddress,
        (byte)ConfigModbusMeterId.Baudrate,
        (byte)ConfigModbusMeterId.SerialConfig,
        (byte)ConfigModbusMeterId.ReadFuncCode,
        (byte)ConfigModbusMeterId.ForwardTotalEnable,
        (byte)ConfigModbusMeterId.ForwardTotalRegAddr,
        (byte)ConfigModbusMeterId.ForwardTotalDataType,
        (byte)ConfigModbusMeterId.ForwardTotalWordSwap,
        (byte)ConfigModbusMeterId.ForwardTotalMultiplier,
        (byte)ConfigModbusMeterId.ReverseTotalEnable,
        (byte)ConfigModbusMeterId.ReverseTotalRegAddr,
        (byte)ConfigModbusMeterId.ReverseTotalDataType,
        (byte)ConfigModbusMeterId.ReverseTotalWordSwap,
        (byte)ConfigModbusMeterId.ReverseTotalMultiplier,
        (byte)ConfigModbusMeterId.FlowRateEnable,
        (byte)ConfigModbusMeterId.FlowRateRegAddr,
        (byte)ConfigModbusMeterId.FlowRateDataType,
        (byte)ConfigModbusMeterId.FlowRateWordSwap,
        (byte)ConfigModbusMeterId.FlowRateMultiplier
    };

            txtReadModbus3UseSetting.Clear();
            txtReadModbus3SerialSetting.Clear();
            txtReadModbus3SlaveAddrSetting.Clear();
            txtReadModbus3BaudSetting.Clear();
            txtReadModbus3FrameFormatSetting.Clear();
            txtReadModbus3FunctionCodeSetting.Clear();
            txtReadForward3UseSetting.Clear();
            txtReadForward3RegAddrSetting.Clear();
            txtReadForward3DataTypeSetting.Clear();
            txtReadForward3WordSwapSetting.Clear();
            txtReadForward3MultiplierSetting.Clear();
            txtReadReverse3UseSetting.Clear();
            txtReadReverse3RegAddrSetting.Clear();
            txtReadReverse3DataTypeSetting.Clear();
            txtReadReverse3WordSwapSetting.Clear();
            txtReadReverse3MultiplierSetting.Clear();
            txtReadFlow3UseSetting.Clear();
            txtReadFlow3RegAddrSetting.Clear();
            txtReadFlow3DataTypeSetting.Clear();
            txtReadFlow3WordSwapSetting.Clear();
            txtReadFlow3MultiplierSetting.Clear();

            GetCommands.ModbusMeter(2, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.ModbusMeter(rxFrame, out ModbusMeterConfig modbusMeterConfig))
            {
                if (!modbusMeterConfig.MeterEnable)
                {
                    txtReadModbus3UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadModbus3UseSetting.Text = "Đang sử dụng";
                    txtReadModbus3SerialSetting.Text = modbusMeterConfig.SerialNumber;
                    txtReadModbus3SlaveAddrSetting.Text = modbusMeterConfig.SlaveAddress.ToString();
                    txtReadModbus3BaudSetting.Text = modbusMeterConfig.BaudRate.ToString();
                    switch (modbusMeterConfig.SerialConfig)
                    {
                        case ((byte)ModbusSerialConfig.Modbus_8N1):
                            txtReadModbus3FrameFormatSetting.Text = "8N1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8O1):
                            txtReadModbus3FrameFormatSetting.Text = "8O1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8E1):
                            txtReadModbus3FrameFormatSetting.Text = "8E1";
                            break;

                        default:
                            break;
                    }
                    txtReadModbus3FunctionCodeSetting.Text = $"0x{modbusMeterConfig.ReadFuncCode:X2}";

                    if (!modbusMeterConfig.ForwardTotal.ParameterEnable)
                    {
                        txtReadForward3UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadForward3UseSetting.Text = "Đang sử dụng";
                        txtReadForward3RegAddrSetting.Text = modbusMeterConfig.ForwardTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ForwardTotal.DataType != 0)
                        {
                            txtReadForward3DataTypeSetting.Text = cmbForward3DataTypeSetting.Items[modbusMeterConfig.ForwardTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ForwardTotal.WordSwap)
                        {
                            txtReadForward3WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadForward3WordSwapSetting.Text = "Không";
                        }
                        txtReadForward3MultiplierSetting.Text = modbusMeterConfig.ForwardTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.ReverseTotal.ParameterEnable)
                    {
                        txtReadReverse3UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadReverse3UseSetting.Text = "Đang sử dụng";
                        txtReadReverse3RegAddrSetting.Text = modbusMeterConfig.ReverseTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ReverseTotal.DataType != 0)
                        {
                            txtReadReverse3DataTypeSetting.Text = cmbReverse3DataTypeSetting.Items[modbusMeterConfig.ReverseTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ReverseTotal.WordSwap)
                        {
                            txtReadReverse3WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadReverse3WordSwapSetting.Text = "Không";
                        }
                        txtReadReverse3MultiplierSetting.Text = modbusMeterConfig.ReverseTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.FlowRate.ParameterEnable)
                    {
                        txtReadFlow3UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadFlow3UseSetting.Text = "Đang sử dụng";
                        txtReadFlow3RegAddrSetting.Text = modbusMeterConfig.FlowRate.RegisterAddress.ToString();
                        if (modbusMeterConfig.FlowRate.DataType != 0)
                        {
                            txtReadFlow3DataTypeSetting.Text = cmbFlow3DataTypeSetting.Items[modbusMeterConfig.FlowRate.DataType].ToString();
                        }
                        if (modbusMeterConfig.FlowRate.WordSwap)
                        {
                            txtReadFlow3WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadFlow3WordSwapSetting.Text = "Không";
                        }
                        txtReadFlow3MultiplierSetting.Text = modbusMeterConfig.FlowRate.Multiplier.ToString();
                    }
                }
            }
        }

        private async void btnWriteModbusMeter3Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkModbus3UseSetting.Checked;

            string serialNumber = txtWriteModbus3SerialSetting.Text.Trim();
            byte? slaveAddress = null;
            uint? baudRate = null;
            byte? serialConfig = null;
            byte? readFuncCode = null;

            bool? forwardEnable = null;
            ushort? forwardRegAddr = null;
            byte? forwardDataType = null;
            bool? forwardWordSwap = null;
            sbyte? forwardMultiplier = null;

            bool? reverseEnable = null;
            ushort? reverseRegAddr = null;
            byte? reverseDataType = null;
            bool? reverseWordSwap = null;
            sbyte? reverseMultiplier = null;

            bool? flowEnable = null;
            ushort? flowRegAddr = null;
            byte? flowDataType = null;
            bool? flowWordSwap = null;
            sbyte? flowMultiplier = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblModbusMeter3SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!byte.TryParse(txtWriteModbus3SlaveAddrSetting.Text, out byte slaveAddressValue))
                {
                    lblModbusMeter3SettingStatus.Text = "Slave Address không hợp lệ.";
                    return;
                }

                if (!uint.TryParse(cmbModbus3BaudSetting.Text, out uint baudRateValue))
                {
                    lblModbusMeter3SettingStatus.Text = "Baud Rate không hợp lệ.";
                    return;
                }

                if (cmbModbus3FrameFormatSetting.SelectedIndex == 0)
                {
                    lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Frame Format.";
                    return;
                }

                if (cmbModbus3FunctionCodeSetting.SelectedIndex == 0)
                {
                    lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }

                slaveAddress = slaveAddressValue;
                baudRate = baudRateValue;
                switch (cmbModbus3FrameFormatSetting.Text)
                {
                    case "8N1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8N1;
                        break;

                    case "8O1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8O1;
                        break;

                    case "8E1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8E1;
                        break;

                    default:
                        lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Frame Format.";
                        return;
                }
                if (!byte.TryParse(cmbModbus3FunctionCodeSetting.Text.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out byte functionCode))
                {
                    lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }
                readFuncCode = functionCode;

                forwardEnable = chkForward3UseSetting.Checked;
                reverseEnable = chkReverse3UseSetting.Checked;
                flowEnable = chkFlow3UseSetting.Checked;

                if (forwardEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteForward3RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter3SettingStatus.Text = "Forward Register Address không hợp lệ.";
                        return;
                    }

                    forwardRegAddr = value;

                    if (cmbForward3DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Forward Data Type.";
                        return;
                    }

                    forwardDataType = (byte)cmbForward3DataTypeSetting.SelectedIndex;
                    forwardWordSwap = cmbForward3WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteForward3MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter3SettingStatus.Text = "Forward Multiplier không hợp lệ.";
                        return;
                    }

                    forwardMultiplier = multiplier;
                }

                if (reverseEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteReverse3RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter3SettingStatus.Text = "Reverse Register Address không hợp lệ.";
                        return;
                    }

                    reverseRegAddr = value;

                    if (cmbReverse3DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Reverse Data Type.";
                        return;
                    }

                    reverseDataType = (byte)cmbReverse3DataTypeSetting.SelectedIndex;
                    reverseWordSwap = cmbReverse3WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteReverse3MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter3SettingStatus.Text = "Reverse Multiplier không hợp lệ.";
                        return;
                    }

                    reverseMultiplier = multiplier;
                }

                if (flowEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteFlow3RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter3SettingStatus.Text = "Flow Rate Register Address không hợp lệ.";
                        return;
                    }

                    flowRegAddr = value;

                    if (cmbFlow3DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter3SettingStatus.Text = "Vui lòng chọn Flow Rate Data Type.";
                        return;
                    }

                    flowDataType = (byte)cmbFlow3DataTypeSetting.SelectedIndex;
                    flowWordSwap = cmbFlow3WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteFlow3MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter3SettingStatus.Text = "Flow Rate Multiplier không hợp lệ.";
                        return;
                    }

                    flowMultiplier = multiplier;
                }
            }

            if (SetCommands.ModbusMeter(
                2,
                enabled,
                serialNumber,
                slaveAddress,
                baudRate,
                serialConfig,
                readFuncCode,
                forwardEnable,
                forwardRegAddr,
                forwardDataType,
                forwardWordSwap,
                forwardMultiplier,
                reverseEnable,
                reverseRegAddr,
                reverseDataType,
                reverseWordSwap,
                reverseMultiplier,
                flowEnable,
                flowRegAddr,
                flowDataType,
                flowWordSwap,
                flowMultiplier,
                out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                if (ok && SetParser.ModbusMeter(rxFrame))
                {
                    lblModbusMeter3SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblModbusMeter3SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblModbusMeter3SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblModbusMeter3SettingStatus.Text = string.Empty;
        }

        private void chkModbus4UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter4Control();
        }

        private void chkForward4UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter4Control();
        }

        private void chkReverse4UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter4Control();
        }

        private void chkFlow4UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModbusMeter4Control();
        }

        private void btnModbusMeter4CopySetting_Click(object sender, EventArgs e)
        {
            chkModbus4UseSetting.Checked = txtReadModbus4UseSetting.Text == "Đang sử dụng";

            txtWriteModbus4SerialSetting.Text = txtReadModbus4SerialSetting.Text;
            txtWriteModbus4SlaveAddrSetting.Text = txtReadModbus4SlaveAddrSetting.Text;

            cmbModbus4BaudSetting.Text = txtReadModbus4BaudSetting.Text;
            cmbModbus4FrameFormatSetting.Text = txtReadModbus4FrameFormatSetting.Text;
            cmbModbus4FunctionCodeSetting.Text = txtReadModbus4FunctionCodeSetting.Text;

            chkForward4UseSetting.Checked = txtReadForward4UseSetting.Text == "Đang sử dụng";
            txtWriteForward4RegAddrSetting.Text = txtReadForward4RegAddrSetting.Text;
            cmbForward4DataTypeSetting.Text = txtReadForward4DataTypeSetting.Text;
            cmbForward4WordSwapSetting.Text = txtReadForward4WordSwapSetting.Text;
            txtWriteForward4MultiplierSetting.Text = txtReadForward4MultiplierSetting.Text;

            chkReverse4UseSetting.Checked = txtReadReverse4UseSetting.Text == "Đang sử dụng";
            txtWriteReverse4RegAddrSetting.Text = txtReadReverse4RegAddrSetting.Text;
            cmbReverse4DataTypeSetting.Text = txtReadReverse4DataTypeSetting.Text;
            cmbReverse4WordSwapSetting.Text = txtReadReverse4WordSwapSetting.Text;
            txtWriteReverse4MultiplierSetting.Text = txtReadReverse4MultiplierSetting.Text;

            chkFlow4UseSetting.Checked = txtReadFlow4UseSetting.Text == "Đang sử dụng";
            txtWriteFlow4RegAddrSetting.Text = txtReadFlow4RegAddrSetting.Text;
            cmbFlow4DataTypeSetting.Text = txtReadFlow4DataTypeSetting.Text;
            cmbFlow4WordSwapSetting.Text = txtReadFlow4WordSwapSetting.Text;
            txtWriteFlow4MultiplierSetting.Text = txtReadFlow4MultiplierSetting.Text;

            UpdateModbusMeter4Control();
        }

        private async void btnReadModbusMeter4Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
        (byte)ConfigModbusMeterId.MeterEnable,
        (byte)ConfigModbusMeterId.SerialNumber,
        (byte)ConfigModbusMeterId.SlaveAddress,
        (byte)ConfigModbusMeterId.Baudrate,
        (byte)ConfigModbusMeterId.SerialConfig,
        (byte)ConfigModbusMeterId.ReadFuncCode,
        (byte)ConfigModbusMeterId.ForwardTotalEnable,
        (byte)ConfigModbusMeterId.ForwardTotalRegAddr,
        (byte)ConfigModbusMeterId.ForwardTotalDataType,
        (byte)ConfigModbusMeterId.ForwardTotalWordSwap,
        (byte)ConfigModbusMeterId.ForwardTotalMultiplier,
        (byte)ConfigModbusMeterId.ReverseTotalEnable,
        (byte)ConfigModbusMeterId.ReverseTotalRegAddr,
        (byte)ConfigModbusMeterId.ReverseTotalDataType,
        (byte)ConfigModbusMeterId.ReverseTotalWordSwap,
        (byte)ConfigModbusMeterId.ReverseTotalMultiplier,
        (byte)ConfigModbusMeterId.FlowRateEnable,
        (byte)ConfigModbusMeterId.FlowRateRegAddr,
        (byte)ConfigModbusMeterId.FlowRateDataType,
        (byte)ConfigModbusMeterId.FlowRateWordSwap,
        (byte)ConfigModbusMeterId.FlowRateMultiplier
    };

            txtReadModbus4UseSetting.Clear();
            txtReadModbus4SerialSetting.Clear();
            txtReadModbus4SlaveAddrSetting.Clear();
            txtReadModbus4BaudSetting.Clear();
            txtReadModbus4FrameFormatSetting.Clear();
            txtReadModbus4FunctionCodeSetting.Clear();
            txtReadForward4UseSetting.Clear();
            txtReadForward4RegAddrSetting.Clear();
            txtReadForward4DataTypeSetting.Clear();
            txtReadForward4WordSwapSetting.Clear();
            txtReadForward4MultiplierSetting.Clear();
            txtReadReverse4UseSetting.Clear();
            txtReadReverse4RegAddrSetting.Clear();
            txtReadReverse4DataTypeSetting.Clear();
            txtReadReverse4WordSwapSetting.Clear();
            txtReadReverse4MultiplierSetting.Clear();
            txtReadFlow4UseSetting.Clear();
            txtReadFlow4RegAddrSetting.Clear();
            txtReadFlow4DataTypeSetting.Clear();
            txtReadFlow4WordSwapSetting.Clear();
            txtReadFlow4MultiplierSetting.Clear();

            GetCommands.ModbusMeter(3, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.ModbusMeter(rxFrame, out ModbusMeterConfig modbusMeterConfig))
            {
                if (!modbusMeterConfig.MeterEnable)
                {
                    txtReadModbus4UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadModbus4UseSetting.Text = "Đang sử dụng";
                    txtReadModbus4SerialSetting.Text = modbusMeterConfig.SerialNumber;
                    txtReadModbus4SlaveAddrSetting.Text = modbusMeterConfig.SlaveAddress.ToString();
                    txtReadModbus4BaudSetting.Text = modbusMeterConfig.BaudRate.ToString();
                    switch (modbusMeterConfig.SerialConfig)
                    {
                        case ((byte)ModbusSerialConfig.Modbus_8N1):
                            txtReadModbus4FrameFormatSetting.Text = "8N1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8O1):
                            txtReadModbus4FrameFormatSetting.Text = "8O1";
                            break;

                        case ((byte)ModbusSerialConfig.Modbus_8E1):
                            txtReadModbus4FrameFormatSetting.Text = "8E1";
                            break;

                        default:
                            break;
                    }
                    txtReadModbus4FunctionCodeSetting.Text = $"0x{modbusMeterConfig.ReadFuncCode:X2}";

                    if (!modbusMeterConfig.ForwardTotal.ParameterEnable)
                    {
                        txtReadForward4UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadForward4UseSetting.Text = "Đang sử dụng";
                        txtReadForward4RegAddrSetting.Text = modbusMeterConfig.ForwardTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ForwardTotal.DataType != 0)
                        {
                            txtReadForward4DataTypeSetting.Text = cmbForward4DataTypeSetting.Items[modbusMeterConfig.ForwardTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ForwardTotal.WordSwap)
                        {
                            txtReadForward4WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadForward4WordSwapSetting.Text = "Không";
                        }
                        txtReadForward4MultiplierSetting.Text = modbusMeterConfig.ForwardTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.ReverseTotal.ParameterEnable)
                    {
                        txtReadReverse4UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadReverse4UseSetting.Text = "Đang sử dụng";
                        txtReadReverse4RegAddrSetting.Text = modbusMeterConfig.ReverseTotal.RegisterAddress.ToString();
                        if (modbusMeterConfig.ReverseTotal.DataType != 0)
                        {
                            txtReadReverse4DataTypeSetting.Text = cmbReverse4DataTypeSetting.Items[modbusMeterConfig.ReverseTotal.DataType].ToString();
                        }
                        if (modbusMeterConfig.ReverseTotal.WordSwap)
                        {
                            txtReadReverse4WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadReverse4WordSwapSetting.Text = "Không";
                        }
                        txtReadReverse4MultiplierSetting.Text = modbusMeterConfig.ReverseTotal.Multiplier.ToString();
                    }

                    if (!modbusMeterConfig.FlowRate.ParameterEnable)
                    {
                        txtReadFlow4UseSetting.Text = "Không sử dụng";
                    }
                    else
                    {
                        txtReadFlow4UseSetting.Text = "Đang sử dụng";
                        txtReadFlow4RegAddrSetting.Text = modbusMeterConfig.FlowRate.RegisterAddress.ToString();
                        if (modbusMeterConfig.FlowRate.DataType != 0)
                        {
                            txtReadFlow4DataTypeSetting.Text = cmbFlow4DataTypeSetting.Items[modbusMeterConfig.FlowRate.DataType].ToString();
                        }
                        if (modbusMeterConfig.FlowRate.WordSwap)
                        {
                            txtReadFlow4WordSwapSetting.Text = "Có";
                        }
                        else
                        {
                            txtReadFlow4WordSwapSetting.Text = "Không";
                        }
                        txtReadFlow4MultiplierSetting.Text = modbusMeterConfig.FlowRate.Multiplier.ToString();
                    }
                }
            }
        }

        private async void btnWriteModbusMeter4Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkModbus4UseSetting.Checked;

            string serialNumber = txtWriteModbus4SerialSetting.Text.Trim();
            byte? slaveAddress = null;
            uint? baudRate = null;
            byte? serialConfig = null;
            byte? readFuncCode = null;

            bool? forwardEnable = null;
            ushort? forwardRegAddr = null;
            byte? forwardDataType = null;
            bool? forwardWordSwap = null;
            sbyte? forwardMultiplier = null;

            bool? reverseEnable = null;
            ushort? reverseRegAddr = null;
            byte? reverseDataType = null;
            bool? reverseWordSwap = null;
            sbyte? reverseMultiplier = null;

            bool? flowEnable = null;
            ushort? flowRegAddr = null;
            byte? flowDataType = null;
            bool? flowWordSwap = null;
            sbyte? flowMultiplier = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblModbusMeter4SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                if (!byte.TryParse(txtWriteModbus4SlaveAddrSetting.Text, out byte slaveAddressValue))
                {
                    lblModbusMeter4SettingStatus.Text = "Slave Address không hợp lệ.";
                    return;
                }

                if (!uint.TryParse(cmbModbus4BaudSetting.Text, out uint baudRateValue))
                {
                    lblModbusMeter4SettingStatus.Text = "Baud Rate không hợp lệ.";
                    return;
                }

                if (cmbModbus4FrameFormatSetting.SelectedIndex == 0)
                {
                    lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Frame Format.";
                    return;
                }

                if (cmbModbus4FunctionCodeSetting.SelectedIndex == 0)
                {
                    lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }

                slaveAddress = slaveAddressValue;
                baudRate = baudRateValue;
                switch (cmbModbus4FrameFormatSetting.Text)
                {
                    case "8N1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8N1;
                        break;

                    case "8O1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8O1;
                        break;

                    case "8E1":
                        serialConfig = (byte)ModbusSerialConfig.Modbus_8E1;
                        break;

                    default:
                        lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Frame Format.";
                        return;
                }
                if (!byte.TryParse(cmbModbus4FunctionCodeSetting.Text.Replace("0x", ""), System.Globalization.NumberStyles.HexNumber, null, out byte functionCode))
                {
                    lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Function Code.";
                    return;
                }
                readFuncCode = functionCode;

                forwardEnable = chkForward4UseSetting.Checked;
                reverseEnable = chkReverse4UseSetting.Checked;
                flowEnable = chkFlow4UseSetting.Checked;

                if (forwardEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteForward4RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter4SettingStatus.Text = "Forward Register Address không hợp lệ.";
                        return;
                    }

                    forwardRegAddr = value;

                    if (cmbForward4DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Forward Data Type.";
                        return;
                    }

                    forwardDataType = (byte)cmbForward4DataTypeSetting.SelectedIndex;
                    forwardWordSwap = cmbForward4WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteForward4MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter4SettingStatus.Text = "Forward Multiplier không hợp lệ.";
                        return;
                    }

                    forwardMultiplier = multiplier;
                }

                if (reverseEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteReverse4RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter4SettingStatus.Text = "Reverse Register Address không hợp lệ.";
                        return;
                    }

                    reverseRegAddr = value;

                    if (cmbReverse4DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Reverse Data Type.";
                        return;
                    }

                    reverseDataType = (byte)cmbReverse4DataTypeSetting.SelectedIndex;
                    reverseWordSwap = cmbReverse4WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteReverse4MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter4SettingStatus.Text = "Reverse Multiplier không hợp lệ.";
                        return;
                    }

                    reverseMultiplier = multiplier;
                }

                if (flowEnable.Value)
                {
                    if (!ushort.TryParse(txtWriteFlow4RegAddrSetting.Text, out ushort value))
                    {
                        lblModbusMeter4SettingStatus.Text = "Flow Rate Register Address không hợp lệ.";
                        return;
                    }

                    flowRegAddr = value;

                    if (cmbFlow4DataTypeSetting.SelectedIndex == 0)
                    {
                        lblModbusMeter4SettingStatus.Text = "Vui lòng chọn Flow Rate Data Type.";
                        return;
                    }

                    flowDataType = (byte)cmbFlow4DataTypeSetting.SelectedIndex;
                    flowWordSwap = cmbFlow4WordSwapSetting.SelectedIndex != 0;

                    if (!sbyte.TryParse(txtWriteFlow4MultiplierSetting.Text, out sbyte multiplier))
                    {
                        lblModbusMeter4SettingStatus.Text = "Flow Rate Multiplier không hợp lệ.";
                        return;
                    }

                    flowMultiplier = multiplier;
                }
            }

            if (SetCommands.ModbusMeter(
                3,
                enabled,
                serialNumber,
                slaveAddress,
                baudRate,
                serialConfig,
                readFuncCode,
                forwardEnable,
                forwardRegAddr,
                forwardDataType,
                forwardWordSwap,
                forwardMultiplier,
                reverseEnable,
                reverseRegAddr,
                reverseDataType,
                reverseWordSwap,
                reverseMultiplier,
                flowEnable,
                flowRegAddr,
                flowDataType,
                flowWordSwap,
                flowMultiplier,
                out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                if (ok && SetParser.ModbusMeter(rxFrame))
                {
                    lblModbusMeter4SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblModbusMeter4SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblModbusMeter4SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblModbusMeter4SettingStatus.Text = string.Empty;
        }


        //-----------------------Pressure Sensor Setting--------------------------------//

        private void chkPressure1UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePressureSensor1Control();
        }
        private void btnPressureSensor1CopySetting_Click(object sender, EventArgs e)
        {
            chkPressure1UseSetting.Checked = txtReadPressure1UseSetting.Text == "Đang sử dụng";

            txtWritePressure1SerialSetting.Text = txtReadPressure1SerialSetting.Text;
            txtWritePressure1MinCurrentSetting.Text = txtReadPressure1MinCurrentSetting.Text;
            txtWritePressure1MaxCurrentSetting.Text = txtReadPressure1MaxCurrentSetting.Text;
            txtWritePressure1MinPressureSetting.Text = txtReadPressure1MinPressureSetting.Text;
            txtWritePressure1MaxPressureSetting.Text = txtReadPressure1MaxPressureSetting.Text;

            UpdatePressureSensor1Control();
        }
        private async void btnReadPressureSensor1Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
        (byte)ConfigPressureSensorId.MeterEnable,
        (byte)ConfigPressureSensorId.SerialNumber,
        (byte)ConfigPressureSensorId.MinCurrent,
        (byte)ConfigPressureSensorId.MaxCurrent,
        (byte)ConfigPressureSensorId.MinPressure,
        (byte)ConfigPressureSensorId.MaxPressure
    };

            txtReadPressure1UseSetting.Clear();
            txtReadPressure1SerialSetting.Clear();
            txtReadPressure1MinCurrentSetting.Clear();
            txtReadPressure1MaxCurrentSetting.Clear();
            txtReadPressure1MinPressureSetting.Clear();
            txtReadPressure1MaxPressureSetting.Clear();

            GetCommands.PressureSensor(0, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.PressureSensor(rxFrame, out PressureSensorConfig pressureSensorConfig))
            {
                if (!pressureSensorConfig.SensorEnable)
                {
                    txtReadPressure1UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadPressure1UseSetting.Text = "Đang sử dụng";
                    txtReadPressure1SerialSetting.Text = pressureSensorConfig.SerialNumber;
                    txtReadPressure1MinCurrentSetting.Text = pressureSensorConfig.MinCurrent.ToString();
                    txtReadPressure1MaxCurrentSetting.Text = pressureSensorConfig.MaxCurrent.ToString();
                    txtReadPressure1MinPressureSetting.Text = pressureSensorConfig.MinPressure.ToString();
                    txtReadPressure1MaxPressureSetting.Text = pressureSensorConfig.MaxPressure.ToString();
                }
            }
        }
        private async void btnWritePressureSensor1Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkPressure1UseSetting.Checked;

            string serialNumber = txtWritePressure1SerialSetting.Text.Trim();
            float? minCurrent = null;
            float? maxCurrent = null;
            float? minPressure = null;
            float? maxPressure = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblPressureSensor1SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                string minCurrentText = txtWritePressure1MinCurrentSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(minCurrentText, out float minCurrentValue))
                {
                    lblPressureSensor1SettingStatus.Text = "Min Current không hợp lệ.";
                    return;
                }

                string maxCurrentText = txtWritePressure1MaxCurrentSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(maxCurrentText, out float maxCurrentValue))
                {
                    lblPressureSensor1SettingStatus.Text = "Max Current không hợp lệ.";
                    return;
                }

                string minPressureText = txtWritePressure1MinPressureSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(minPressureText, out float minPressureValue))
                {
                    lblPressureSensor1SettingStatus.Text = "Min Pressure không hợp lệ.";
                    return;
                }

                string maxPressureText = txtWritePressure1MaxPressureSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(maxPressureText, out float maxPressureValue))
                {
                    lblPressureSensor1SettingStatus.Text = "Max Pressure không hợp lệ.";
                    return;
                }

                if (minCurrentValue >= maxCurrentValue)
                {
                    lblPressureSensor1SettingStatus.Text = "Min Current phải nhỏ hơn Max Current.";
                    return;
                }

                if (minPressureValue >= maxPressureValue)
                {
                    lblPressureSensor1SettingStatus.Text = "Min Pressure phải nhỏ hơn Max Pressure.";
                    return;
                }

                minCurrent = minCurrentValue;
                maxCurrent = maxCurrentValue;
                minPressure = minPressureValue;
                maxPressure = maxPressureValue;
            }

            if (SetCommands.PressureSensor(0, enabled, serialNumber, minCurrent, maxCurrent, minPressure, maxPressure, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                if (ok && SetParser.PressureSensor(rxFrame))
                {
                    lblPressureSensor1SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblPressureSensor1SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblPressureSensor1SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblPressureSensor1SettingStatus.Text = string.Empty;
        }


        private void chkPressure2UseSetting_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePressureSensor2Control();
        }
        private void btnPressureSensor2CopySetting_Click(object sender, EventArgs e)
        {
            chkPressure2UseSetting.Checked = txtReadPressure2UseSetting.Text == "Đang sử dụng";

            txtWritePressure2SerialSetting.Text = txtReadPressure2SerialSetting.Text;
            txtWritePressure2MinCurrentSetting.Text = txtReadPressure2MinCurrentSetting.Text;
            txtWritePressure2MaxCurrentSetting.Text = txtReadPressure2MaxCurrentSetting.Text;
            txtWritePressure2MinPressureSetting.Text = txtReadPressure2MinPressureSetting.Text;
            txtWritePressure2MaxPressureSetting.Text = txtReadPressure2MaxPressureSetting.Text;

            UpdatePressureSensor2Control();
        }
        private async void btnReadPressureSensor2Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] parameterIds =
            {
        (byte)ConfigPressureSensorId.MeterEnable,
        (byte)ConfigPressureSensorId.SerialNumber,
        (byte)ConfigPressureSensorId.MinCurrent,
        (byte)ConfigPressureSensorId.MaxCurrent,
        (byte)ConfigPressureSensorId.MinPressure,
        (byte)ConfigPressureSensorId.MaxPressure
    };

            txtReadPressure2UseSetting.Clear();
            txtReadPressure2SerialSetting.Clear();
            txtReadPressure2MinCurrentSetting.Clear();
            txtReadPressure2MaxCurrentSetting.Clear();
            txtReadPressure2MinPressureSetting.Clear();
            txtReadPressure2MaxPressureSetting.Clear();

            GetCommands.PressureSensor(1, parameterIds, out txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.PressureSensor(rxFrame, out PressureSensorConfig pressureSensorConfig))
            {
                if (!pressureSensorConfig.SensorEnable)
                {
                    txtReadPressure2UseSetting.Text = "Không sử dụng";
                }
                else
                {
                    txtReadPressure2UseSetting.Text = "Đang sử dụng";
                    txtReadPressure2SerialSetting.Text = pressureSensorConfig.SerialNumber;
                    txtReadPressure2MinCurrentSetting.Text = pressureSensorConfig.MinCurrent.ToString();
                    txtReadPressure2MaxCurrentSetting.Text = pressureSensorConfig.MaxCurrent.ToString();
                    txtReadPressure2MinPressureSetting.Text = pressureSensorConfig.MinPressure.ToString();
                    txtReadPressure2MaxPressureSetting.Text = pressureSensorConfig.MaxPressure.ToString();
                }
            }
        }
        private async void btnWritePressureSensor2Setting_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            bool enabled = chkPressure2UseSetting.Checked;

            string serialNumber = txtWritePressure2SerialSetting.Text.Trim();
            float? minCurrent = null;
            float? maxCurrent = null;
            float? minPressure = null;
            float? maxPressure = null;

            if (enabled)
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    lblPressureSensor2SettingStatus.Text = "Serial Number không được để trống.";
                    return;
                }

                string minCurrentText = txtWritePressure2MinCurrentSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(minCurrentText, out float minCurrentValue))
                {
                    lblPressureSensor2SettingStatus.Text = "Min Current không hợp lệ.";
                    return;
                }

                string maxCurrentText = txtWritePressure2MaxCurrentSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(maxCurrentText, out float maxCurrentValue))
                {
                    lblPressureSensor2SettingStatus.Text = "Max Current không hợp lệ.";
                    return;
                }

                string minPressureText = txtWritePressure2MinPressureSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(minPressureText, out float minPressureValue))
                {
                    lblPressureSensor2SettingStatus.Text = "Min Pressure không hợp lệ.";
                    return;
                }

                string maxPressureText = txtWritePressure2MaxPressureSetting.Text.Trim().Replace('.', ',');
                if (!float.TryParse(maxPressureText, out float maxPressureValue))
                {
                    lblPressureSensor2SettingStatus.Text = "Max Pressure không hợp lệ.";
                    return;
                }

                if (minCurrentValue >= maxCurrentValue)
                {
                    lblPressureSensor2SettingStatus.Text = "Min Current phải nhỏ hơn Max Current.";
                    return;
                }

                if (minPressureValue >= maxPressureValue)
                {
                    lblPressureSensor2SettingStatus.Text = "Min Pressure phải nhỏ hơn Max Pressure.";
                    return;
                }

                minCurrent = minCurrentValue;
                maxCurrent = maxCurrentValue;
                minPressure = minPressureValue;
                maxPressure = maxPressureValue;
            }

            if (SetCommands.PressureSensor(1, enabled, serialNumber, minCurrent, maxCurrent, minPressure, maxPressure, out txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                if (ok && SetParser.PressureSensor(rxFrame))
                {
                    lblPressureSensor2SettingStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblPressureSensor2SettingStatus.Text = string.Empty;
                    return;
                }
            }

            lblPressureSensor2SettingStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblPressureSensor2SettingStatus.Text = string.Empty;
        }



        //-----------------------Query Latch Data--------------------------------//

        private async void btnReadLatchData_Click(object sender, EventArgs e)
        {
            if (!ushort.TryParse(txtBeginIndexLatchQuery.Text.Trim(), out ushort beginUserIndex))
            {
                if (!string.IsNullOrWhiteSpace(txtBeginIndexLatchQuery.Text))
                {
                    MessageBox.Show("Begin Index không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                beginUserIndex = 1;
            }

            if (beginUserIndex < 1)
            {
                MessageBox.Show("Begin Index phải lớn hơn hoặc bằng 1.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool hasEndIndex = ushort.TryParse(txtEndIndexLatchQuery.Text.Trim(), out ushort endUserIndex);

            if (!string.IsNullOrWhiteSpace(txtEndIndexLatchQuery.Text) && !hasEndIndex)
            {
                MessageBox.Show("End Index không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (hasEndIndex && endUserIndex < 1)
            {
                MessageBox.Show("End Index phải lớn hơn hoặc bằng 1.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (hasEndIndex && beginUserIndex > endUserIndex)
            {
                MessageBox.Show("Begin Index phải nhỏ hơn hoặc bằng End Index.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvLatchInfo.Rows.Clear();

            _stopReadLatchData = false;
            btnReadLatchData.Enabled = false;
            btnStopReadLatchData.Enabled = true;

            try
            {
                for (uint userIndex = beginUserIndex; ; userIndex++)
                {
                    if (_stopReadLatchData)
                    {
                        break;
                    }

                    if (hasEndIndex && userIndex > endUserIndex)
                    {
                        break;
                    }

                    ushort latchIndex = (ushort)(userIndex - 1);

                    QueryCommands.Latch(latchIndex, out byte[] txFrame);

                    var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                    if (_stopReadLatchData)
                    {
                        break;
                    }

                    if (!ok)
                    {
                        break;
                    }

                    if (!QueryParser.Latch(rxFrame, out LatchData latchData))
                    {
                        break;
                    }

                    int rowIndex = dgvLatchInfo.Rows.Add(userIndex, latchData.LatchDateTime.ToString("dd/MM/yyyy HH:mm:ss"));
                    dgvLatchInfo.Rows[rowIndex].Tag = latchData;

                    if (userIndex >= ushort.MaxValue)
                    {
                        break;
                    }
                }
            }
            finally
            {
                btnReadLatchData.Enabled = true;
                btnStopReadLatchData.Enabled = false;
            }
        }

        private void dgvLatchInfo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (!(dgvLatchInfo.Rows[e.RowIndex].Tag is LatchData latchData))
            {
                return;
            }

            DisplayLatchDetail(latchData);
        }

        private void btnStopReadLatchData_Click(object sender, EventArgs e)
        {
            _stopReadLatchData = true;
        }



        //-----------------------Query Event Data--------------------------------//

        private async void btnReadEventData_Click(object sender, EventArgs e)
        {
            if (!ushort.TryParse(txtBeginIndexEventQuery.Text.Trim(), out ushort beginUserIndex))
            {
                if (!string.IsNullOrWhiteSpace(txtBeginIndexEventQuery.Text))
                {
                    MessageBox.Show("Begin Index không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                beginUserIndex = 1;
            }

            bool hasEndIndex = ushort.TryParse(txtEndIndexEventQuery.Text.Trim(), out ushort endUserIndex);

            if (!string.IsNullOrWhiteSpace(txtEndIndexEventQuery.Text) && !hasEndIndex)
            {
                MessageBox.Show("End Index không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (hasEndIndex && beginUserIndex > endUserIndex)
            {
                MessageBox.Show("Begin Index phải nhỏ hơn hoặc bằng End Index.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvEventData.Rows.Clear();

            _stopReadEventData = false;
            btnReadEventData.Enabled = false;
            btnStopReadEventData.Enabled = true;

            try
            {
                for (uint userIndex = beginUserIndex; ; userIndex++)
                {
                    if (_stopReadEventData)
                    {
                        break;
                    }

                    if (hasEndIndex && userIndex > endUserIndex)
                    {
                        break;
                    }

                    ushort eventIndex = (ushort)(userIndex - 1);

                    if (!QueryCommands.Event(eventIndex, out byte[] txFrame))
                    {
                        break;
                    }

                    var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

                    if (_stopReadEventData)
                    {
                        break;
                    }

                    if (!ok)
                    {
                        break;
                    }

                    if (!QueryParser.Event(rxFrame, out EventData eventData))
                    {
                        break;
                    }

                    string meterType;

                    switch (eventData.MeterType)
                    {
                        case 0:
                            meterType = "Module";
                            break;

                        case 1:
                            meterType = "Pulse";
                            break;

                        case 2:
                            meterType = "Modbus";
                            break;

                        case 3:
                            meterType = "Pressure";
                            break;

                        default:
                            meterType = "-";
                            break;
                    }

                    int rowIndex = dgvEventData.Rows.Add(
                        userIndex,
                        meterType,
                        eventData.SerialNumber,
                        eventData.EventCode.ToString(),
                        eventData.EventDateTime.ToString("dd/MM/yyyy HH:mm:ss"));

                    dgvEventData.Rows[rowIndex].Tag = eventData;

                    if (userIndex >= ushort.MaxValue)
                    {
                        break;
                    }
                }
            }
            finally
            {
                btnReadEventData.Enabled = true;
                btnStopReadEventData.Enabled = false;
            }
        }

        private void btnStopReadEventData_Click(object sender, EventArgs e)
        {
            _stopReadEventData = true;
        }

        //-----------------------Query Metadata--------------------------------//
        private async void btnReadMetadata_Click(object sender, EventArgs e)
        {
            QueryCommands.Metadata(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.Metadata(rxFrame, out MetadataData metadata))
            {
                txtMetadataSeq.Text = metadata.SequenceMeta.ToString();
                txtNextSaveLatchIndex.Text = metadata.NextLatchSaveIndex.ToString();
                txtNextLoadLatchIndex.Text = metadata.NextLatchLoadIndex.ToString();
                txtLatchCount.Text = metadata.LatchCount.ToString();
                txtNextSaveEventIndex.Text = metadata.NextEventSaveIndex.ToString();
                txtNextLoadEventIndex.Text = metadata.NextEventLoadIndex.ToString();
                txtEventCount.Text = metadata.EventCount.ToString();
                txtNextSaveErrLogIndex.Text = metadata.NextLogSaveIndex.ToString();
                txtErrLogCount.Text = metadata.LogCount.ToString();

                txtRuntimeSeq.Text = metadata.SequenceRuntime.ToString();
                txtPulseForwardTotal1.Text = metadata.PulseCounts[0].ForwardPulseCount.ToString();
                txtPulseReverseTotal1.Text = metadata.PulseCounts[0].ReversePulseCount.ToString();
                txtPulseForwardTotal2.Text = metadata.PulseCounts[1].ForwardPulseCount.ToString();
                txtPulseReverseTotal2.Text = metadata.PulseCounts[1].ReversePulseCount.ToString();
                txtPulseForwardTotal3.Text = metadata.PulseCounts[2].ForwardPulseCount.ToString();
                txtPulseReverseTotal3.Text = metadata.PulseCounts[2].ReversePulseCount.ToString();
                txtPulseForwardTotal4.Text = metadata.PulseCounts[3].ForwardPulseCount.ToString();
                txtPulseReverseTotal4.Text = metadata.PulseCounts[3].ReversePulseCount.ToString();
            }
        }

        //----------------------------Change password----------------------------------------//

        private async void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Text) ||
                string.IsNullOrWhiteSpace(txtNewPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                lblChangePasswordStatus.Text = "Không được để trống.";
                await Task.Delay(1000);
                lblChangePasswordStatus.Text = string.Empty;
                return;
            }

            if (cmbRoleNewPassword.SelectedIndex == 0)
            {
                lblChangePasswordStatus.Text = "Vui lòng chọn quyền.";
                await Task.Delay(1000);
                lblChangePasswordStatus.Text = string.Empty;
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblChangePasswordStatus.Text = "Mật khẩu xác nhận không khớp.";
                await Task.Delay(1000);
                lblChangePasswordStatus.Text = string.Empty;
                return;
            }

            if (!SetCommands.ChangePassword(txtCurrentPassword.Text, (byte)cmbRoleNewPassword.SelectedIndex, txtNewPassword.Text, out byte[] txFrame))
            {
                lblChangePasswordStatus.Text = "Thất bại.";
                await Task.Delay(1000);
                lblChangePasswordStatus.Text = string.Empty;
                return;
            }

            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);

            if (ok && SetParser.ChangePassword(rxFrame))
            {
                lblChangePasswordStatus.Text = "Thành công";
            }
            else
            {
                lblChangePasswordStatus.Text = "Thất bại";
            }

            await Task.Delay(1000);
            lblChangePasswordStatus.Text = string.Empty;
        }


        //----------------------------OTA----------------------------------------//

        private void btnBrowseOtaFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Chọn Firmware";
                openFileDialog.Filter = "Firmware (*.bin)|*.bin|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFirmwareFile.Text = openFileDialog.FileName;
                }
            }
        }

        private async void btnFirmwareUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                _stopFirmwareUpdate = false;
                prgOtaProgress.Value = 0;

                string filePath = txtFirmwareFile.Text.Trim();

                if (string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show("Chưa chọn file firmware.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file firmware.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                btnFirmwareUpdate.Enabled = false;

                // Read firmware file
                byte[] firmwareFile = await Task.Run(() => File.ReadAllBytes(filePath));

                // Read Metadata A
                if (!ReadFirmwareMetadata(firmwareFile, OTA_METADATA_A_OFFSET, out FirmwareMetadata metadataA))
                {
                    MessageBox.Show("Không đọc được Firmware Metadata A.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read Metadata B
                if (!ReadFirmwareMetadata(firmwareFile, OTA_METADATA_B_OFFSET, out FirmwareMetadata metadataB))
                {
                    MessageBox.Show("Không đọc được Firmware Metadata B.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Compare firmware version
                if (metadataA.Version != metadataB.Version)
                {
                    MessageBox.Show(
                        $"Firmware version không hợp lệ.\r\n\r\nMetadata A: {metadataA.Version}\r\nMetadata B: {metadataB.Version}",
                        "OTA",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                // Get Firmware A
                if (!GetFirmwareData(firmwareFile, OTA_FIRMWARE_A_OFFSET, metadataA.Size, out byte[] firmwareA))
                {
                    MessageBox.Show("Không đọc được Firmware A.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get Firmware B
                if (!GetFirmwareData(firmwareFile, OTA_FIRMWARE_B_OFFSET, metadataB.Size, out byte[] firmwareB))
                {
                    MessageBox.Show("Không đọc được Firmware B.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Send OTA Request
                if (!OtaCommands.Request(metadataA.Version, metadataA.Size, metadataA.Crc, metadataB.Size, metadataB.Crc, out byte[] requestFrame))
                {
                    MessageBox.Show("Không tạo được OTA Request frame.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var (ok, requestResponse) = await _serialPortManager.CommunicateAsync(requestFrame, 5000);

                if (!ok)
                {
                    MessageBox.Show("Không nhận được ACK từ thiết bị.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Parse OTA Request response
                if (!OtaParser.Request(requestResponse, out byte otaSlot, out ushort packetIndex))
                {
                    MessageBox.Show("OTA Request response không hợp lệ.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (otaSlot == 0)
                {
                    MessageBox.Show("Thiết bị không chấp nhận cập nhật firmware.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Select firmware according to OTA slot
                byte[] firmware;

                if (otaSlot == 1)
                {
                    firmware = firmwareA;
                }
                else if (otaSlot == 2)
                {
                    firmware = firmwareB;
                }
                else
                {
                    MessageBox.Show($"OTA Slot không hợp lệ: {otaSlot}.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Calculate packet count
                int totalPacketCount = (firmware.Length + OTA_PACKET_SIZE - 1) / OTA_PACKET_SIZE;

                if (packetIndex >= totalPacketCount)
                {
                    MessageBox.Show("Packet index từ thiết bị không hợp lệ.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                prgOtaProgress.Minimum = 0;
                prgOtaProgress.Maximum = totalPacketCount;
                prgOtaProgress.Value = packetIndex;

                // Send firmware packets
                while (packetIndex < totalPacketCount)
                {
                    if (_stopFirmwareUpdate)
                    {
                        MessageBox.Show("Đã dừng cập nhật firmware.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    int offset = packetIndex * OTA_PACKET_SIZE;
                    int packetSize = Math.Min(OTA_PACKET_SIZE, firmware.Length - offset);

                    byte[] packet = new byte[packetSize];
                    Array.Copy(firmware, offset, packet, 0, packetSize);

                    if (!OtaCommands.SendPacket(packetIndex, packet, out byte[] packetFrame))
                    {
                        MessageBox.Show($"Không tạo được packet {packetIndex}.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var (packetOk, packetResponse) = await _serialPortManager.CommunicateAsync(packetFrame, 5000);

                    if (!packetOk)
                    {
                        MessageBox.Show($"Không nhận được ACK packet {packetIndex}.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!OtaParser.SendPacket(packetResponse))
                    {
                        MessageBox.Show($"Packet {packetIndex} cập nhật thất bại.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    packetIndex++;

                    prgOtaProgress.Value = Math.Min(packetIndex, totalPacketCount);
                }

                MessageBox.Show("Cập nhật firmware thành công.", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi OTA:\r\n{ex.Message}", "OTA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnFirmwareUpdate.Enabled = true;
            }
        }

        private void btnStopFirmwareUpdate_Click(object sender, EventArgs e)
        {
            _stopFirmwareUpdate = true;
        }


        //-----------------------Advanced Setting--------------------------------//

        private async void btnReadSerialAdvCfg_Click(object sender, EventArgs e)
        {
            GetCommands.ModuleSerial(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok)
            {
                ulong serial = GetParser.ModuleSerial(rxFrame);
                txtReadSerialAdvCfg.Text = serial.ToString("D12");
            }
        }

        private async void btnWriteSerialAdvCfg_Click(object sender, EventArgs e)
        {
            if (SetCommands.ModuleSerial(txtWriteSerialAdvCfg.Text, out byte[] txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
                if (ok && SetParser.ModuleSerial(rxFrame))
                {
                    lblWriteSerialStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblWriteSerialStatus.Text = string.Empty;
                    return;
                }
            }

            lblWriteSerialStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblWriteSerialStatus.Text = string.Empty;
            return;
        }

        private async void btnReadMcuReset_Click(object sender, EventArgs e)
        {
            GetCommands.McuResetCount(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.McuResetCount(rxFrame, out byte count))
            {
                txtReadMcuReset.Text = count.ToString();
            }
        }

        private async void btnWriteMcuReset_Click(object sender, EventArgs e)
        {
            if (SetCommands.McuResetCount(txtWriteMcuReset.Text, out byte[] txFrame))
            {
                var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
                if (ok && SetParser.McuResetCount(rxFrame))
                {
                    lblWriteMcuResetStatus.Text = "Ghi thành công";
                    await Task.Delay(1000);
                    lblWriteMcuResetStatus.Text = string.Empty;
                    return;
                }
            }

            lblWriteMcuResetStatus.Text = "Ghi thất bại";
            await Task.Delay(1000);
            lblWriteMcuResetStatus.Text = string.Empty;
            return;
        }

        private async void btnRebootDevice_Click(object sender, EventArgs e)
        {
            SetCommands.Reboot(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnResetConfig_Click(object sender, EventArgs e)
        {
            SetCommands.ResetSetting(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            SetCommands.ResetPassword(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnEraseLatchData_Click(object sender, EventArgs e)
        {
            SetCommands.EraseLatchData(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnEraseEventData_Click(object sender, EventArgs e)
        {
            SetCommands.EraseEventData(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnEraseErrLog_Click(object sender, EventArgs e)
        {
            SetCommands.EraseErrLog(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnLatchActive_Click(object sender, EventArgs e)
        {
            SetCommands.LatchActive(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnWriteEvent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEventMeterIndex.Text))
                return;

            byte value = byte.Parse(txtEventMeterIndex.Text);
            SetCommands.EventCreate((byte)cmbEventMeter.SelectedIndex, value, (byte)cmbEventCreate.SelectedIndex, out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnPushActive_Click(object sender, EventArgs e)
        {
            SetCommands.PushActive(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
        }

        private async void btnFactoryReset_Click(object sender, EventArgs e)
        {
            SetCommands.FactoryReset(out byte[] txFrame);
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok)
            {
                ulong serial = GetParser.ModuleSerial(rxFrame);
                txtModuleSerialOverall.Text = serial.ToString("D12");
            }

            // Read Bootloader version
            QueryCommands.BootloaderVersion(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.BootloaderVersion(rxFrame, out string bootVer))
            {
                txtBootloaderVersion.Text = bootVer;
            }

            // Read Firmware version
            QueryCommands.FirmwareVersion(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.FirmwareVersion(rxFrame, out string fwVer))
            {
                txtFirmwareVersion.Text = fwVer;
            }

            // Read Date Time
            GetCommands.DateTime(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.DateTime(rxFrame, out DateTime dateTime))
            {
                txtTimeOverall.Text = dateTime.ToString("dd/MM/yyyy HH:mm:ss");
            }

            // Read IP Endpoint
            GetCommands.IpEndpoint(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && GetParser.IpEndpoint(rxFrame, out string ip, out string port))
            {
                txtIPV4Overall.Text = ip;
                txtPortOverall.Text = port;
            }

            // Read Latch Period
            GetCommands.LatchPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok)
            {
                ushort latchPeriod = GetParser.LatchPeriod(rxFrame);
                txtLatchPeriodOverall.Text = latchPeriod.ToString();
            }

            // Read Push Period
            GetCommands.PushPeriod(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok)
            {
                ushort PushPeriod = GetParser.PushPeriod(rxFrame);
                txtPushPeriodOverall.Text = PushPeriod.ToString();
            }

            // Read Timezone
            GetCommands.Timezone(out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial1Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward1Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse1Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal1Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate1Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.PulseMeterData(1, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial2Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward2Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse2Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal2Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate2Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.PulseMeterData(2, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PulseMeterData(rxFrame, out meterData))
            {
                txtPulseMeterSerial3Overall.Text = Encoding.ASCII.GetString(meterData.MeterSerial).TrimEnd('\0');
                txtPulseMeterForward3Overall.Text = meterData.ForwardTotalizer.ToString();
                txtPulseMeterReverse3Overall.Text = meterData.ReverseTotalizer.ToString();
                txtPulseMeterRealTotal3Overall.Text = (meterData.ForwardTotalizer - meterData.ReverseTotalizer).ToString();
                txtPulseMeterFlowRate3Overall.Text = meterData.FlowRate.ToString();
            }

            QueryCommands.PulseMeterData(3, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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
            var (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
            if (ok && QueryParser.PressureSensorData(rxFrame, out meterSerial, out pressure))
            {
                txtPressureSensorSerial1Overall.Text = Encoding.ASCII.GetString(meterSerial).TrimEnd('\0');
                txtPressure1Overall.Text = pressure.ToString();
            }

            QueryCommands.PressureSensorData(1, out txFrame);
            (ok, rxFrame) = await _serialPortManager.CommunicateAsync(txFrame, 5000);
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

        private void UpdateModbusMeter1Control()
        {
            bool enabled = chkModbus1UseSetting.Checked;
            txtWriteModbus1SerialSetting.Enabled = enabled;
            txtWriteModbus1SlaveAddrSetting.Enabled = enabled;
            cmbModbus1BaudSetting.Enabled = enabled;
            cmbModbus1FrameFormatSetting.Enabled = enabled;
            cmbModbus1FunctionCodeSetting.Enabled = enabled;
            chkForward1UseSetting.Enabled = enabled;
            txtWriteForward1RegAddrSetting.Enabled = enabled && chkForward1UseSetting.Checked;
            cmbForward1DataTypeSetting.Enabled = enabled && chkForward1UseSetting.Checked;
            cmbForward1WordSwapSetting.Enabled = enabled && chkForward1UseSetting.Checked;
            txtWriteForward1MultiplierSetting.Enabled = enabled && chkForward1UseSetting.Checked;
            chkReverse1UseSetting.Enabled = enabled;
            txtWriteReverse1RegAddrSetting.Enabled = enabled && chkReverse1UseSetting.Checked;
            cmbReverse1DataTypeSetting.Enabled = enabled && chkReverse1UseSetting.Checked;
            cmbReverse1WordSwapSetting.Enabled = enabled && chkReverse1UseSetting.Checked;
            txtWriteReverse1MultiplierSetting.Enabled = enabled && chkReverse1UseSetting.Checked;
            chkFlow1UseSetting.Enabled = enabled;
            txtWriteFlow1RegAddrSetting.Enabled = enabled && chkFlow1UseSetting.Checked;
            cmbFlow1DataTypeSetting.Enabled = enabled && chkFlow1UseSetting.Checked;
            cmbFlow1WordSwapSetting.Enabled = enabled && chkFlow1UseSetting.Checked;
            txtWriteFlow1MultiplierSetting.Enabled = enabled && chkFlow1UseSetting.Checked;
        }

        private void UpdateModbusMeter2Control()
        {
            bool enabled = chkModbus2UseSetting.Checked;
            txtWriteModbus2SerialSetting.Enabled = enabled;
            txtWriteModbus2SlaveAddrSetting.Enabled = enabled;
            cmbModbus2BaudSetting.Enabled = enabled;
            cmbModbus2FrameFormatSetting.Enabled = enabled;
            cmbModbus2FunctionCodeSetting.Enabled = enabled;
            chkForward2UseSetting.Enabled = enabled;
            txtWriteForward2RegAddrSetting.Enabled = enabled && chkForward2UseSetting.Checked;
            cmbForward2DataTypeSetting.Enabled = enabled && chkForward2UseSetting.Checked;
            cmbForward2WordSwapSetting.Enabled = enabled && chkForward2UseSetting.Checked;
            txtWriteForward2MultiplierSetting.Enabled = enabled && chkForward2UseSetting.Checked;
            chkReverse2UseSetting.Enabled = enabled;
            txtWriteReverse2RegAddrSetting.Enabled = enabled && chkReverse2UseSetting.Checked;
            cmbReverse2DataTypeSetting.Enabled = enabled && chkReverse2UseSetting.Checked;
            cmbReverse2WordSwapSetting.Enabled = enabled && chkReverse2UseSetting.Checked;
            txtWriteReverse2MultiplierSetting.Enabled = enabled && chkReverse2UseSetting.Checked;
            chkFlow2UseSetting.Enabled = enabled;
            txtWriteFlow2RegAddrSetting.Enabled = enabled && chkFlow2UseSetting.Checked;
            cmbFlow2DataTypeSetting.Enabled = enabled && chkFlow2UseSetting.Checked;
            cmbFlow2WordSwapSetting.Enabled = enabled && chkFlow2UseSetting.Checked;
            txtWriteFlow2MultiplierSetting.Enabled = enabled && chkFlow2UseSetting.Checked;
        }

        private void UpdateModbusMeter3Control()
        {
            bool enabled = chkModbus3UseSetting.Checked;
            txtWriteModbus3SerialSetting.Enabled = enabled;
            txtWriteModbus3SlaveAddrSetting.Enabled = enabled;
            cmbModbus3BaudSetting.Enabled = enabled;
            cmbModbus3FrameFormatSetting.Enabled = enabled;
            cmbModbus3FunctionCodeSetting.Enabled = enabled;
            chkForward3UseSetting.Enabled = enabled;
            txtWriteForward3RegAddrSetting.Enabled = enabled && chkForward3UseSetting.Checked;
            cmbForward3DataTypeSetting.Enabled = enabled && chkForward3UseSetting.Checked;
            cmbForward3WordSwapSetting.Enabled = enabled && chkForward3UseSetting.Checked;
            txtWriteForward3MultiplierSetting.Enabled = enabled && chkForward3UseSetting.Checked;
            chkReverse3UseSetting.Enabled = enabled;
            txtWriteReverse3RegAddrSetting.Enabled = enabled && chkReverse3UseSetting.Checked;
            cmbReverse3DataTypeSetting.Enabled = enabled && chkReverse3UseSetting.Checked;
            cmbReverse3WordSwapSetting.Enabled = enabled && chkReverse3UseSetting.Checked;
            txtWriteReverse3MultiplierSetting.Enabled = enabled && chkReverse3UseSetting.Checked;
            chkFlow3UseSetting.Enabled = enabled;
            txtWriteFlow3RegAddrSetting.Enabled = enabled && chkFlow3UseSetting.Checked;
            cmbFlow3DataTypeSetting.Enabled = enabled && chkFlow3UseSetting.Checked;
            cmbFlow3WordSwapSetting.Enabled = enabled && chkFlow3UseSetting.Checked;
            txtWriteFlow3MultiplierSetting.Enabled = enabled && chkFlow3UseSetting.Checked;
        }

        private void UpdateModbusMeter4Control()
        {
            bool enabled = chkModbus4UseSetting.Checked;
            txtWriteModbus4SerialSetting.Enabled = enabled;
            txtWriteModbus4SlaveAddrSetting.Enabled = enabled;
            cmbModbus4BaudSetting.Enabled = enabled;
            cmbModbus4FrameFormatSetting.Enabled = enabled;
            cmbModbus4FunctionCodeSetting.Enabled = enabled;
            chkForward4UseSetting.Enabled = enabled;
            txtWriteForward4RegAddrSetting.Enabled = enabled && chkForward4UseSetting.Checked;
            cmbForward4DataTypeSetting.Enabled = enabled && chkForward4UseSetting.Checked;
            cmbForward4WordSwapSetting.Enabled = enabled && chkForward4UseSetting.Checked;
            txtWriteForward4MultiplierSetting.Enabled = enabled && chkForward4UseSetting.Checked;
            chkReverse4UseSetting.Enabled = enabled;
            txtWriteReverse4RegAddrSetting.Enabled = enabled && chkReverse4UseSetting.Checked;
            cmbReverse4DataTypeSetting.Enabled = enabled && chkReverse4UseSetting.Checked;
            cmbReverse4WordSwapSetting.Enabled = enabled && chkReverse4UseSetting.Checked;
            txtWriteReverse4MultiplierSetting.Enabled = enabled && chkReverse4UseSetting.Checked;
            chkFlow4UseSetting.Enabled = enabled;
            txtWriteFlow4RegAddrSetting.Enabled = enabled && chkFlow4UseSetting.Checked;
            cmbFlow4DataTypeSetting.Enabled = enabled && chkFlow4UseSetting.Checked;
            cmbFlow4WordSwapSetting.Enabled = enabled && chkFlow4UseSetting.Checked;
            txtWriteFlow4MultiplierSetting.Enabled = enabled && chkFlow4UseSetting.Checked;
        }

        private void UpdatePressureSensor1Control()
        {
            bool enabled = chkPressure1UseSetting.Checked;
            txtWritePressure1SerialSetting.Enabled = enabled;
            txtWritePressure1MinCurrentSetting.Enabled = enabled;
            txtWritePressure1MaxCurrentSetting.Enabled = enabled;
            txtWritePressure1MinPressureSetting.Enabled = enabled;
            txtWritePressure1MaxPressureSetting.Enabled = enabled;
        }

        private void UpdatePressureSensor2Control()
        {
            bool enabled = chkPressure2UseSetting.Checked;
            txtWritePressure2SerialSetting.Enabled = enabled;
            txtWritePressure2MinCurrentSetting.Enabled = enabled;
            txtWritePressure2MaxCurrentSetting.Enabled = enabled;
            txtWritePressure2MinPressureSetting.Enabled = enabled;
            txtWritePressure2MaxPressureSetting.Enabled = enabled;
        }

        private void DisplayLatchDetail(LatchData latchData)
        {
            lblLatchDateTime.Text = $"Chi tiết thời gian chốt: {latchData.LatchDateTime:dd/MM/yyyy HH:mm:ss}";

            dgvLatchMeterDetail.Rows.Clear();

            for (int i = 0; i < latchData.Meters.Count; i++)
            {
                LatchMeterData meter = latchData.Meters[i];

                string meterType;
                string forward;
                string reverse;
                string flowRate;
                string pressure;

                switch (meter.MeterType)
                {
                    case (byte)MeterType.PulseMeterType:
                        meterType = "Pulse";
                        forward = meter.ForwardTotalizer.ToString();
                        reverse = meter.ReverseTotalizer.ToString();
                        flowRate = meter.FlowRate.ToString();
                        pressure = "-";
                        break;

                    case (byte)MeterType.ModbusMeterType:
                        meterType = "Modbus";
                        forward = meter.ForwardTotalizer.ToString();
                        reverse = meter.ReverseTotalizer.ToString();
                        flowRate = meter.FlowRate.ToString();
                        pressure = "-";
                        break;

                    case (byte)MeterType.PressureSensorType:
                        meterType = "Pressure";
                        forward = "-";
                        reverse = "-";
                        flowRate = "-";
                        pressure = meter.Pressure.ToString();
                        break;

                    default:
                        meterType = "-";
                        forward = "-";
                        reverse = "-";
                        flowRate = "-";
                        pressure = "-";
                        break;
                }

                dgvLatchMeterDetail.Rows.Add(i + 1, meterType, meter.SerialNumber, forward, reverse, flowRate, pressure);
            }
        }

        private bool ReadFirmwareMetadata(byte[] firmwareFile, int offset, out FirmwareMetadata metadata)
        {
            metadata = null;

            if (firmwareFile == null || offset < 0 || offset + OTA_METADATA_SIZE > firmwareFile.Length)
            {
                return false;
            }

            int index = offset;

            uint sequence = BitConverter.ToUInt32(firmwareFile, index);
            index += sizeof(uint);

            byte[] versionBytes = new byte[12];
            Array.Copy(firmwareFile, index, versionBytes, 0, versionBytes.Length);
            index += versionBytes.Length;

            index += sizeof(uint); // Firmware_Status_t

            uint size = BitConverter.ToUInt32(firmwareFile, index);
            index += sizeof(uint);

            uint crc = BitConverter.ToUInt32(firmwareFile, index);

            metadata = new FirmwareMetadata
            {
                Sequence = sequence,
                Version = Encoding.ASCII.GetString(versionBytes).TrimEnd('\0'),
                Size = size,
                Crc = crc
            };

            return true;
        }

        private bool GetFirmwareData(byte[] firmwareFile, int offset, uint size, out byte[] firmware)
        {
            firmware = null;

            if (size == 0 || size > int.MaxValue)
            {
                return false;
            }

            if (offset < 0 || offset + (long)size > firmwareFile.Length)
            {
                return false;
            }

            firmware = new byte[size];

            Array.Copy(firmwareFile, offset, firmware, 0, (int)size);

            return true;
        }
    }
}