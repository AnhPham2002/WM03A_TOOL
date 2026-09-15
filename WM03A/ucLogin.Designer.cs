namespace WM03A
{
    partial class ucLogin
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtModulePassword = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblModulePassword = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnRefreshCom = new System.Windows.Forms.Button();
            this.btnOpenCom = new System.Windows.Forms.Button();
            this.lblSelectCom = new System.Windows.Forms.Label();
            this.cmbRole = new System.Windows.Forms.ComboBox();
            this.cmbCom = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtModulePassword
            // 
            this.txtModulePassword.Location = new System.Drawing.Point(273, 403);
            this.txtModulePassword.Name = "txtModulePassword";
            this.txtModulePassword.PasswordChar = '*';
            this.txtModulePassword.Size = new System.Drawing.Size(300, 20);
            this.txtModulePassword.TabIndex = 13;
            this.txtModulePassword.UseSystemPasswordChar = true;
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Location = new System.Drawing.Point(107, 323);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(37, 13);
            this.lblRole.TabIndex = 11;
            this.lblRole.Text = "Vai trò";
            // 
            // lblModulePassword
            // 
            this.lblModulePassword.AutoSize = true;
            this.lblModulePassword.Location = new System.Drawing.Point(107, 406);
            this.lblModulePassword.Name = "lblModulePassword";
            this.lblModulePassword.Size = new System.Drawing.Size(135, 13);
            this.lblModulePassword.TabIndex = 12;
            this.lblModulePassword.Text = "Nhập mật khẩu của thiết bị";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(600, 401);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(94, 23);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "Đăng nhập";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnRefreshCom
            // 
            this.btnRefreshCom.Location = new System.Drawing.Point(600, 210);
            this.btnRefreshCom.Name = "btnRefreshCom";
            this.btnRefreshCom.Size = new System.Drawing.Size(94, 23);
            this.btnRefreshCom.TabIndex = 9;
            this.btnRefreshCom.Text = "Làm mới COM";
            this.btnRefreshCom.UseVisualStyleBackColor = true;
            this.btnRefreshCom.Click += new System.EventHandler(this.btnRefreshCom_Click);
            // 
            // btnOpenCom
            // 
            this.btnOpenCom.Location = new System.Drawing.Point(719, 210);
            this.btnOpenCom.Name = "btnOpenCom";
            this.btnOpenCom.Size = new System.Drawing.Size(75, 23);
            this.btnOpenCom.TabIndex = 10;
            this.btnOpenCom.Text = "Mở COM";
            this.btnOpenCom.UseVisualStyleBackColor = true;
            this.btnOpenCom.Click += new System.EventHandler(this.btnOpenCom_Click);
            // 
            // lblSelectCom
            // 
            this.lblSelectCom.AutoSize = true;
            this.lblSelectCom.Location = new System.Drawing.Point(107, 217);
            this.lblSelectCom.Name = "lblSelectCom";
            this.lblSelectCom.Size = new System.Drawing.Size(59, 13);
            this.lblSelectCom.TabIndex = 7;
            this.lblSelectCom.Text = "Chọn COM";
            // 
            // cmbRole
            // 
            this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRole.FormattingEnabled = true;
            this.cmbRole.Items.AddRange(new object[] {
            "Người xem",
            "Kỹ thuật viên",
            "Quản trị viên"});
            this.cmbRole.Location = new System.Drawing.Point(172, 320);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new System.Drawing.Size(401, 21);
            this.cmbRole.TabIndex = 5;
            // 
            // cmbCom
            // 
            this.cmbCom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCom.FormattingEnabled = true;
            this.cmbCom.Location = new System.Drawing.Point(172, 212);
            this.cmbCom.Name = "cmbCom";
            this.cmbCom.Size = new System.Drawing.Size(401, 21);
            this.cmbCom.TabIndex = 6;
            // 
            // ucLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtModulePassword);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.lblModulePassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnRefreshCom);
            this.Controls.Add(this.btnOpenCom);
            this.Controls.Add(this.lblSelectCom);
            this.Controls.Add(this.cmbRole);
            this.Controls.Add(this.cmbCom);
            this.Name = "ucLogin";
            this.Size = new System.Drawing.Size(960, 750);
            this.Load += new System.EventHandler(this.ucLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtModulePassword;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblModulePassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRefreshCom;
        private System.Windows.Forms.Button btnOpenCom;
        private System.Windows.Forms.Label lblSelectCom;
        private System.Windows.Forms.ComboBox cmbRole;
        private System.Windows.Forms.ComboBox cmbCom;
    }
}
