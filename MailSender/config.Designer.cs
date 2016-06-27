namespace MailSender
{
    partial class configForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(configForm));
            this.lb_email = new System.Windows.Forms.Label();
            this.tb_email = new System.Windows.Forms.TextBox();
            this.lb_pwd = new System.Windows.Forms.Label();
            this.tb_pwd = new System.Windows.Forms.TextBox();
            this.lb_saveEmail = new System.Windows.Forms.Label();
            this.cb_true = new System.Windows.Forms.CheckBox();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lb_email
            // 
            this.lb_email.AutoSize = true;
            this.lb_email.Location = new System.Drawing.Point(40, 39);
            this.lb_email.Name = "lb_email";
            this.lb_email.Size = new System.Drawing.Size(47, 12);
            this.lb_email.TabIndex = 0;
            this.lb_email.Text = "邮 箱：";
            // 
            // tb_email
            // 
            this.tb_email.Location = new System.Drawing.Point(86, 36);
            this.tb_email.Name = "tb_email";
            this.tb_email.Size = new System.Drawing.Size(200, 21);
            this.tb_email.TabIndex = 1;
            // 
            // lb_pwd
            // 
            this.lb_pwd.AutoSize = true;
            this.lb_pwd.Location = new System.Drawing.Point(40, 98);
            this.lb_pwd.Name = "lb_pwd";
            this.lb_pwd.Size = new System.Drawing.Size(47, 12);
            this.lb_pwd.TabIndex = 2;
            this.lb_pwd.Text = "密 码：";
            // 
            // tb_pwd
            // 
            this.tb_pwd.Location = new System.Drawing.Point(86, 95);
            this.tb_pwd.Name = "tb_pwd";
            this.tb_pwd.Size = new System.Drawing.Size(200, 21);
            this.tb_pwd.TabIndex = 3;
            this.tb_pwd.PasswordChar = '*';
            // 
            // lb_saveEmail
            // 
            this.lb_saveEmail.AutoSize = true;
            this.lb_saveEmail.Location = new System.Drawing.Point(40, 165);
            this.lb_saveEmail.Name = "lb_saveEmail";
            this.lb_saveEmail.Size = new System.Drawing.Size(137, 12);
            this.lb_saveEmail.TabIndex = 4;
            this.lb_saveEmail.Text = "是否保留未发送的邮件：";
            // 
            // cb_true
            // 
            this.cb_true.AutoSize = true;
            this.cb_true.Checked = true;
            this.cb_true.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_true.Location = new System.Drawing.Point(183, 164);
            this.cb_true.Name = "cb_true";
            this.cb_true.Size = new System.Drawing.Size(36, 16);
            this.cb_true.TabIndex = 5;
            this.cb_true.Text = "是";
            this.cb_true.UseVisualStyleBackColor = true;
            // 
            // btn_ok
            // 
            this.btn_ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_ok.Location = new System.Drawing.Point(86, 211);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(72, 28);
            this.btn_ok.TabIndex = 7;
            this.btn_ok.Text = "确 定";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_cancel.Location = new System.Drawing.Point(183, 211);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(72, 28);
            this.btn_cancel.TabIndex = 8;
            this.btn_cancel.Text = "取 消";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // configForm
            // 
            this.AcceptButton = this.btn_ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_cancel;
            this.ClientSize = new System.Drawing.Size(327, 262);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.cb_true);
            this.Controls.Add(this.lb_saveEmail);
            this.Controls.Add(this.tb_pwd);
            this.Controls.Add(this.lb_pwd);
            this.Controls.Add(this.tb_email);
            this.Controls.Add(this.lb_email);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "configForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_email;
        private System.Windows.Forms.TextBox tb_email;
        private System.Windows.Forms.Label lb_pwd;
        private System.Windows.Forms.TextBox tb_pwd;
        private System.Windows.Forms.Label lb_saveEmail;
        private System.Windows.Forms.CheckBox cb_true;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
    }
}