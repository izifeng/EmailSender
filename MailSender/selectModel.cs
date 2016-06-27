using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace MailSender
{
    public partial class selectModel : Form
    {
        public selectModel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int selectIndex = this.comboBox1.SelectedIndex;
            string selectText = this.comboBox1.SelectedItem.ToString();

            //缓存选择的模式
            myUtils.addOrUpdateCache(SysConstant.CACHE_MODEL_INDEX, selectIndex);
            myUtils.addOrUpdateCache(SysConstant.CACHE_MODEL_TEXT, selectText);

            new main().Show();
            this.Hide();
        }

        #region 窗体关闭方法
        /// <summary>
        /// 窗体关闭方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        #endregion

    }
}
