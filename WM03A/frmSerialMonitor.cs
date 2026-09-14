using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WM03A
{
    public partial class frmSerialMonitor : Form
    {
        private SerialPortManager _serialPortManager;
        public frmSerialMonitor(SerialPortManager serialPortManager)
        {
            InitializeComponent();

            _serialPortManager = serialPortManager;

            _serialPortManager.DataSent += SerialPortManager_DataSent;
            _serialPortManager.DataReceived += SerialPortManager_DataReceived;
        }

        private void frmSerialMonitor_Load(object sender, EventArgs e)
        {

        }

        private void SerialPortManager_DataSent(byte[] data)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddTxLog(data)));
                return;
            }

            AddTxLog(data);
        }

        private void SerialPortManager_DataReceived(byte[] data)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddRxLog(data)));
                return;
            }

            AddRxLog(data);
        }

        private void AddTxLog(byte[] data)
        {
            string text = BitConverter.ToString(data).Replace("-", " ");

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = Color.Green;

            rtbLog.AppendText(
                $"{DateTime.Now:HH:mm:ss.fff}  TX  {text}{Environment.NewLine}");

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        private void AddRxLog(byte[] data)
        {
            string text = BitConverter.ToString(data).Replace("-", " ");

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;

            rtbLog.SelectionColor = Color.Red;

            rtbLog.AppendText(
                $"{DateTime.Now:HH:mm:ss.fff}  RX  {text}{Environment.NewLine}");

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.ScrollToCaret();
        }

        private void btnAutoScrollMonitor_Click(object sender, EventArgs e)
        {

        }

        private void btnClearMonitor_Click(object sender, EventArgs e)
        {

        }
    }
}
