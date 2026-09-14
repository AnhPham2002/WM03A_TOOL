namespace WM03A
{
    partial class ucMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCommon = new System.Windows.Forms.TabPage();
            this.tabModuleSetting = new System.Windows.Forms.TabPage();
            this.btnLogout = new System.Windows.Forms.Button();
            this.tabPulseMeterSetting = new System.Windows.Forms.TabPage();
            this.tabModbusMeterSetting = new System.Windows.Forms.TabPage();
            this.tabPressureSensorSetting = new System.Windows.Forms.TabPage();
            this.tabOTA = new System.Windows.Forms.TabPage();
            this.tabChangePassword = new System.Windows.Forms.TabPage();
            this.tabAdvance = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 166);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hello world";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabCommon);
            this.tabControl1.Controls.Add(this.tabModuleSetting);
            this.tabControl1.Controls.Add(this.tabPulseMeterSetting);
            this.tabControl1.Controls.Add(this.tabModbusMeterSetting);
            this.tabControl1.Controls.Add(this.tabPressureSensorSetting);
            this.tabControl1.Controls.Add(this.tabOTA);
            this.tabControl1.Controls.Add(this.tabChangePassword);
            this.tabControl1.Controls.Add(this.tabAdvance);
            this.tabControl1.Location = new System.Drawing.Point(20, 49);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(759, 432);
            this.tabControl1.TabIndex = 1;
            // 
            // tabCommon
            // 
            this.tabCommon.Location = new System.Drawing.Point(4, 22);
            this.tabCommon.Name = "tabCommon";
            this.tabCommon.Padding = new System.Windows.Forms.Padding(3);
            this.tabCommon.Size = new System.Drawing.Size(751, 406);
            this.tabCommon.TabIndex = 0;
            this.tabCommon.Text = "Chung";
            this.tabCommon.UseVisualStyleBackColor = true;
            // 
            // tabModuleSetting
            // 
            this.tabModuleSetting.Location = new System.Drawing.Point(4, 22);
            this.tabModuleSetting.Name = "tabModuleSetting";
            this.tabModuleSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabModuleSetting.Size = new System.Drawing.Size(751, 422);
            this.tabModuleSetting.TabIndex = 1;
            this.tabModuleSetting.Text = "Cài đặt mô-đun";
            this.tabModuleSetting.UseVisualStyleBackColor = true;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(704, 10);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(75, 23);
            this.btnLogout.TabIndex = 0;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // tabPulseMeterSetting
            // 
            this.tabPulseMeterSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPulseMeterSetting.Name = "tabPulseMeterSetting";
            this.tabPulseMeterSetting.Size = new System.Drawing.Size(751, 422);
            this.tabPulseMeterSetting.TabIndex = 2;
            this.tabPulseMeterSetting.Text = "Cài đặt đồng hồ xung";
            this.tabPulseMeterSetting.UseVisualStyleBackColor = true;
            // 
            // tabModbusMeterSetting
            // 
            this.tabModbusMeterSetting.Location = new System.Drawing.Point(4, 22);
            this.tabModbusMeterSetting.Name = "tabModbusMeterSetting";
            this.tabModbusMeterSetting.Size = new System.Drawing.Size(751, 422);
            this.tabModbusMeterSetting.TabIndex = 3;
            this.tabModbusMeterSetting.Text = "Cài đặt đồng hồ modbus";
            this.tabModbusMeterSetting.UseVisualStyleBackColor = true;
            // 
            // tabPressureSensorSetting
            // 
            this.tabPressureSensorSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPressureSensorSetting.Name = "tabPressureSensorSetting";
            this.tabPressureSensorSetting.Size = new System.Drawing.Size(751, 422);
            this.tabPressureSensorSetting.TabIndex = 4;
            this.tabPressureSensorSetting.Text = "Cài đặt cảm biến áp suất";
            this.tabPressureSensorSetting.UseVisualStyleBackColor = true;
            // 
            // tabOTA
            // 
            this.tabOTA.Location = new System.Drawing.Point(4, 22);
            this.tabOTA.Name = "tabOTA";
            this.tabOTA.Size = new System.Drawing.Size(751, 422);
            this.tabOTA.TabIndex = 5;
            this.tabOTA.Text = "Cập nhật phần mềm";
            this.tabOTA.UseVisualStyleBackColor = true;
            // 
            // tabChangePassword
            // 
            this.tabChangePassword.Location = new System.Drawing.Point(4, 22);
            this.tabChangePassword.Name = "tabChangePassword";
            this.tabChangePassword.Size = new System.Drawing.Size(751, 422);
            this.tabChangePassword.TabIndex = 6;
            this.tabChangePassword.Text = "Đổi mật khẩu";
            this.tabChangePassword.UseVisualStyleBackColor = true;
            // 
            // tabAdvance
            // 
            this.tabAdvance.Location = new System.Drawing.Point(4, 22);
            this.tabAdvance.Name = "tabAdvance";
            this.tabAdvance.Size = new System.Drawing.Size(751, 422);
            this.tabAdvance.TabIndex = 7;
            this.tabAdvance.Text = "Cài đặt nâng cao";
            this.tabAdvance.UseVisualStyleBackColor = true;
            // 
            // ucMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label1);
            this.Name = "ucMain";
            this.Size = new System.Drawing.Size(800, 500);
            this.Load += new System.EventHandler(this.ucMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCommon;
        private System.Windows.Forms.TabPage tabModuleSetting;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.TabPage tabPulseMeterSetting;
        private System.Windows.Forms.TabPage tabModbusMeterSetting;
        private System.Windows.Forms.TabPage tabPressureSensorSetting;
        private System.Windows.Forms.TabPage tabOTA;
        private System.Windows.Forms.TabPage tabChangePassword;
        private System.Windows.Forms.TabPage tabAdvance;
    }
}
