using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Models;

namespace MailSender
{
    public partial class configForm : Form
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string configFilePath = Application.StartupPath + SysConstant.FILE_CONFIG;

        public configForm()
        {
            InitializeComponent();
            readConfigByXML();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            string tb_email = this.tb_email.Text.Trim();
            string tb_pwd = this.tb_pwd.Text.Trim();

            bool cb_true = this.cb_true.Checked;

            if (string.IsNullOrWhiteSpace(tb_email)) {
                MessageBoxEx.Show(this,"邮箱地址不能为空!","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            
            if (string.IsNullOrWhiteSpace(tb_pwd)) {
                MessageBox.Show(this, "邮箱密码不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!myUtils.IsEmail(tb_email)) {
                MessageBox.Show(this,"邮箱格式不正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Config cfg = new Config();
            cfg.SenderEmail = tb_email;
            cfg.EmailPwd = tb_pwd;
            if (cb_true) {
                cfg.IsSaveEmail = true;
            }
            else {
                cfg.IsSaveEmail = false;
            }
            //写入配置文件
            bool b = myUtils.XMLWriterByConfig(cfg, Application.StartupPath);
            if (b)
            {
                //把配置保存到 Hashtable中，方便以后的使用
                myUtils.addOrUpdateCache(SysConstant.CACHE_CONFIG,cfg);
            }
            else {
                MessageBox.Show(this,"配置失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 从本地XML文件中获取 配置信息
        /// </summary>
        private void readConfigByXML()
        {
            Config cfg = myUtils.XMLReaderByConfig(configFilePath);
            if (cfg != null) {
                this.tb_email.Text = cfg.SenderEmail;
                this.tb_pwd.Text = cfg.EmailPwd;
                this.cb_true.Checked = cfg.IsSaveEmail;
            }

        }


    }
}
