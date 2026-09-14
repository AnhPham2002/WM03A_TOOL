using System;
using System.Windows.Forms;

namespace WM03A
{
    public partial class ucMain : UserControl
    {
        private Protocol.AccessId _accessId;

        public event EventHandler LogoutRequested;

        public ucMain()
        {
            InitializeComponent();
        }

        public ucMain(Protocol.AccessId accessId)
        {
            InitializeComponent();

            _accessId = accessId;

            ApplyAccessControl();
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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}