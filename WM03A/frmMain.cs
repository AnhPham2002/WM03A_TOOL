using System;
using System.Windows.Forms;

namespace WM03A
{
    public partial class frmMain : Form
    {
        private SerialPortManager _serialPortManager;
        private frmSerialMonitor _serialMonitor;
        private ucLogin _login;

        public frmMain()
        {
            InitializeComponent();

            _serialPortManager = new SerialPortManager();

            _serialMonitor = new frmSerialMonitor(_serialPortManager);
            _serialMonitor.Show();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ShowLogin();
        }

        private void ShowLogin()
        {
            pnlContent.Controls.Clear();

            _login = new ucLogin(_serialPortManager);

            _login.Dock = DockStyle.Fill;
            _login.LoginSucceeded += Login_LoginSucceeded;

            pnlContent.Controls.Add(_login);
        }

        private void Login_LoginSucceeded(object sender, Protocol.AccessId accessId)
        {
            ShowMain(accessId);
        }

        private void ShowMain(Protocol.AccessId accessId)
        {
            pnlContent.Controls.Clear();

            ucMain main = new ucMain(_serialPortManager, accessId, _login);

            main.Dock = DockStyle.Fill;
            main.LogoutRequested += Main_LogoutRequested;

            pnlContent.Controls.Add(main);
        }

        private void Main_LogoutRequested(object sender, EventArgs e)
        {
            ShowLogin();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_serialMonitor != null && !_serialMonitor.IsDisposed)
            {
                _serialMonitor.Close();
            }

            if (_serialPortManager != null)
            {
                _serialPortManager.Close();
            }

            base.OnFormClosing(e);
        }
    }
}