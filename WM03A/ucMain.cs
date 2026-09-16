using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WM03A
{
    public partial class ucMain : UserControl
    {
        private Protocol.AccessId _accessId;

        public event EventHandler LogoutRequested;

        private SerialPortManager _serialPortManager;

        private bool _isReading = false;

        public ucMain(SerialPortManager serialPortManager, Protocol.AccessId accessId)
        {
            InitializeComponent();

            _accessId = accessId;

            ApplyAccessControl();

            _serialPortManager = serialPortManager;
        }

        private void ucMain_Load(object sender, EventArgs e)
        {
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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
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
                txtTimezoneOverall.Text = Timezone.ToString();
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
    }
}