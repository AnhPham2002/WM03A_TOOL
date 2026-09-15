using System;
using System.Threading;
using System.Windows.Forms;
using System.Text;

namespace WM03A
{
    public partial class ucMain : UserControl
    {
        private Protocol.AccessId _accessId;

        public event EventHandler LogoutRequested;

        private SerialPortManager _serialPortManager;

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

        private void btnReadModuleInfo_Click(object sender, EventArgs e)
        {
            byte[] txFrame;
            byte[] rxFrame = null;

            // Read Module Serial
            GetCommands.ModuleSerial(out txFrame);
            if (_serialPortManager.Communicate(txFrame, out rxFrame))
            {
                ulong serial = GetParser.ModuleSerial(rxFrame);
                txtModuleSerialOverall.Text = serial.ToString("D12");
            }

            // Read Date Time
            GetCommands.DateTime(out txFrame);
            if (_serialPortManager.Communicate(txFrame, out rxFrame))
            {
                DateTime dateTime;
                GetParser.DateTime(rxFrame, out dateTime);
                txtTimeOverall.Text = dateTime.ToString("dd/MM/yyyy HH:mm:ss");
            }

            // Read IP Endpoint
            GetCommands.IpEndpoint(out txFrame);
            if (_serialPortManager.Communicate(txFrame, out rxFrame))
            {
                GetParser.IpEndpoint(rxFrame, out string ip, out string port);
                txtIPV4Overall.Text = ip;
                txtPortOverall.Text = port;
            }

            // Read Latch Period
            GetCommands.LatchPeriod(out txFrame);
            if (_serialPortManager.Communicate(txFrame, out rxFrame))
            {
                ushort latchPeriod = GetParser.LatchPeriod(rxFrame);
                txtLatchPeriodOverall.Text = latchPeriod.ToString();
            }

            // Read Push Period
            GetCommands.PushPeriod(out txFrame);
            if (_serialPortManager.Communicate(txFrame, out rxFrame))
            {
                ushort PushPeriod = GetParser.PushPeriod(rxFrame);
                txtPushPeriodOverall.Text = PushPeriod.ToString();
            }

            // Read Timezone
            GetCommands.Timezone(out txFrame);
            if (_serialPortManager.Communicate(txFrame, out rxFrame))
            {
                float Timezone = GetParser.Timezone(rxFrame);
                txtTimezoneOverall.Text = Timezone.ToString();
            }
        }

        private void btnReadInternetStatus_Click(object sender, EventArgs e)
        {

        }

        private void btnReadAllOverall_Click(object sender, EventArgs e)
        {
            btnReadModuleInfo_Click(null, null);
            btnReadInternetStatus_Click(null, null);
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            txtModuleSerialOverall.Clear();
            txtTimeOverall.Clear();

        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }

        private void tabCommon_Click(object sender, EventArgs e)
        {

        }

        private void lblSerial_Click(object sender, EventArgs e)
        {

        }

        private void lblPulseForwardTotal_Click(object sender, EventArgs e)
        {

        }

        private void lblPulseReverseTotal_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox44_TextChanged(object sender, EventArgs e)
        {

        }
    }
}