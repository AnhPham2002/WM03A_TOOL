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
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            ShowLogin();
        }

        private void ShowLogin()
        {
            pnlContent.Controls.Clear();

            ucLogin login = new ucLogin();

            login.Dock = DockStyle.Fill;

            login.LoginSucceeded += Login_LoginSucceeded;

            pnlContent.Controls.Add(login);
        }

        private void Login_LoginSucceeded(object sender, EventArgs e)
        {
            ShowMain();
        }

        private void ShowMain()
        {
            pnlContent.Controls.Clear();

            ucMain main = new ucMain();

            main.Dock = DockStyle.Fill;

            pnlContent.Controls.Add(main);
        }
    }
}
