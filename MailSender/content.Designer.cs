namespace MailSender
{
    partial class contentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(contentForm));
            this.lb_emailTitle = new System.Windows.Forms.Label();
            this.tb_emailTitle = new System.Windows.Forms.TextBox();
            this.lb_emailContent = new System.Windows.Forms.Label();
            this.tb_emailContent = new System.Windows.Forms.TextBox();
            this.btn_sendEmail = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_emailTitle
            // 
            this.lb_emailTitle.AutoSize = true;
            this.lb_emailTitle.Location = new System.Drawing.Point(32, 29);
            this.lb_emailTitle.Name = "lb_emailTitle";
            this.lb_emailTitle.Size = new System.Drawing.Size(47, 12);
            this.lb_emailTitle.TabIndex = 0;
            this.lb_emailTitle.Text = "标 题：";
            // 
            // tb_emailTitle
            // 
            this.tb_emailTitle.Location = new System.Drawing.Point(85, 26);
            this.tb_emailTitle.Name = "tb_emailTitle";
            this.tb_emailTitle.Size = new System.Drawing.Size(450, 21);
            this.tb_emailTitle.TabIndex = 1;
            // 
            // lb_emailContent
            // 
            this.lb_emailContent.AutoSize = true;
            this.lb_emailContent.Location = new System.Drawing.Point(32, 86);
            this.lb_emailContent.Name = "lb_emailContent";
            this.lb_emailContent.Size = new System.Drawing.Size(47, 12);
            this.lb_emailContent.TabIndex = 2;
            this.lb_emailContent.Text = "内 容：";
            // 
            // tb_emailContent
            // 
            this.tb_emailContent.Location = new System.Drawing.Point(85, 86);
            this.tb_emailContent.Multiline = true;
            this.tb_emailContent.Name = "tb_emailContent";
            this.tb_emailContent.Size = new System.Drawing.Size(450, 200);
            this.tb_emailContent.TabIndex = 3;
            // 
            // btn_sendEmail
            // 
            this.btn_sendEmail.Location = new System.Drawing.Point(460, 316);
            this.btn_sendEmail.Name = "btn_sendEmail";
            this.btn_sendEmail.Size = new System.Drawing.Size(75, 26);
            this.btn_sendEmail.TabIndex = 4;
            this.btn_sendEmail.Text = "发 送";
            this.btn_sendEmail.UseVisualStyleBackColor = true;
            this.btn_sendEmail.Click += new System.EventHandler(this.btn_sendEmail_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Crimson;
            this.label1.Location = new System.Drawing.Point(32, 323);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(293, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "注：邮件标题可以为空，邮件内容必须大于10个字符！";
            // 
            // contentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_sendEmail);
            this.Controls.Add(this.tb_emailContent);
            this.Controls.Add(this.lb_emailContent);
            this.Controls.Add(this.tb_emailTitle);
            this.Controls.Add(this.lb_emailTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "contentForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "邮件内容";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_emailTitle;
        private System.Windows.Forms.TextBox tb_emailTitle;
        private System.Windows.Forms.Label lb_emailContent;
        private System.Windows.Forms.TextBox tb_emailContent;
        private System.Windows.Forms.Button btn_sendEmail;
        private System.Windows.Forms.Label label1;
    }
}