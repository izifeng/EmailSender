using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;
using Utils;
using System.Threading;
using System.IO;

namespace MailSender
{
    public partial class contentForm : Form
    {
        private DataGridView grid1;

        /// <summary>
        /// 存放未发送邮件的 email 路径
        /// </summary>
        private string emailFileFolder = Application.StartupPath + SysConstant.FOLDER_FILE;

        /// <summary>
        /// 存放未发送邮件的 email 路径
        /// </summary>
        private string sendFilePath = Application.StartupPath + SysConstant.FOLDER_SEND;

        /// <summary>
        /// 存放未发送邮件的 email 路径
        /// </summary>
        private string emailFilePath = Application.StartupPath + SysConstant.FILE_EMAILS;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string configFilePath = Application.StartupPath + SysConstant.FILE_CONFIG;

        /// <summary>
        /// 存放发送成功的 邮件信息
        /// </summary>
        private List<Email> successEmails = new List<Email>();

        /// <summary>
        /// 存放 发送失败的 邮件信息
        /// </summary>
        private List<Email> failedEmails = new List<Email>();

        private Thread threadSendEmail = null;

        private Config config = null;

        //邮件标题
        private string emailTitle = string.Empty;

        //邮件内容
        private string emailContent = string.Empty;

        public contentForm(DataGridView g)
        {
            this.grid1 = g;
            InitializeComponent();
        }

        private void btn_sendEmail_Click(object sender, EventArgs e)
        {
            ButtonBase btn = (ButtonBase)sender;
            
            //发送邮件逻辑

            //获取需要发送的邮件
            List<Email> emailList = new List<Email>();
            if (myUtils.existsCache(SysConstant.CACHE_EMAILS))
            {
                emailList = (List<Email>)myUtils.getCache(SysConstant.CACHE_EMAILS);
            }
            else
            {
                MessageBoxEx.Show(this, "没有需要发送的邮件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (emailList.Count == 00)
            {
                MessageBoxEx.Show(this, "没有需要发送的邮件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (this.threadSendEmail != null && this.threadSendEmail.IsAlive)
                {
                    this.threadSendEmail.Abort();
                    //结束线程后 修改缓存中线程状态
                    myUtils.deleteCache(SysConstant.CACHE_EMAIL_SEBD_STATE);
                }

                this.Dispose();

                return;
            }


            //邮件内容必须在10字以上，要不怕邮件服务器 把邮件给退回

            this.emailTitle = this.tb_emailTitle.Text.Trim();

            this.emailContent = this.tb_emailContent.Text.Trim();
            if (this.emailContent.Length <= 10)
            {
                MessageBoxEx.Show(this, "邮件内容必须大于10个字符！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            CallBackDelegate cbd = CallBack;


            //新开一个线程来执行发送邮件工作
            this.threadSendEmail = new Thread(new ParameterizedThreadStart(EmailSender));

            this.threadSendEmail.IsBackground = true;

            this.threadSendEmail.Start(cbd);

            this.Close();
        }



        #region 发送邮件
        /// <summary>
        /// 开始发送邮件
        /// </summary>
        private void EmailSender(Object o)
        {
            //把发送邮件线程状态 保存在缓存中，方便在用户退出系统前给出提示
            myUtils.addOrUpdateCache(SysConstant.CACHE_EMAIL_SEBD_STATE, true);

            //获取配置信息
            if (myUtils.existsCache(SysConstant.CACHE_CONFIG))
            {
                this.config = (Config)myUtils.getCache(SysConstant.CACHE_CONFIG);
            }
            else
            {
                this.config = myUtils.XMLReaderByConfig(this.configFilePath);
            }


            //要发送的邮件
            List<Email> emailList = (List<Email>)myUtils.getCache(SysConstant.CACHE_EMAILS);


            foreach (Email mail in emailList)
            {
                MailMsg sm = new MailMsg();
                sm.senderID = this.config.SenderEmail;
                sm.senderPwd = this.config.EmailPwd;
                sm.senderName = this.config.SenderEmail;

                if (string.IsNullOrWhiteSpace(this.emailTitle))
                {
                    sm.emailTitle = mail.emailTitle;
                }
                else
                {
                    sm.emailTitle = this.emailTitle;
                }

                mail.emailTitle = sm.emailTitle;

                //添加附件
                sm.attachments = new List<string>();

                //读取附件
                string fileFolder = sendFilePath + mail.recipientEmail + "/";

                DirectoryInfo folder = new DirectoryInfo(fileFolder);
                foreach (FileInfo NextFile in folder.GetFiles())
                {
                    string fileFullName = NextFile.FullName;
                    //只获取excel文件
                    string fileName = fileFullName.Substring(fileFullName.LastIndexOf("\\") + 1);
                    string fileNameExt = fileName.Substring(fileName.LastIndexOf(".") + 1);

                    string filter = "xls,xlsx";
                    if (filter.Contains(fileNameExt))
                    {
                        sm.attachments.Add(fileFullName);
                    }
                }

                string mailCallPrefix = SysConstant.MAIL_CALL_PREFIX;
                sm.emailContent = "<span>" + mailCallPrefix + " " + mail.recipientName + "：<br/>&nbsp;&nbsp;</span>" + this.emailContent;
                sm.recipientEmail = mail.recipientEmail;
                sm.recipientName = mail.recipientName;

                mail.emailContent = this.emailContent;

                bool b = myUtils.sendEmail(sm);
                if (b)
                {
                    mail.state = "已发送";
                    mail.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                    this.successEmails.Add(mail);
                }
                else
                {
                    mail.state = "发送失败";
                    mail.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                    this.failedEmails.Add(mail);
                }
                //把传来的参数转换为委托
                CallBackDelegate cbd = o as CallBackDelegate;
                //执行回调.
                cbd(emailList.Count,this.successEmails.Count,this.failedEmails.Count);
            }
        }

        /// <summary>
        /// 回调函数
        /// </summary>
        /// <param name="list"></param>
        private void CallBack(int a, int b, int c)
        {
            this.grid1.Invalidate();
            if (a == b + c) {
                Thread.Sleep(500);
                MessageBoxEx.Show(this, "发送完成!一共" + a + "条邮件，完成了" + b + "条，失败了" + c + "条", "提示", MessageBoxButtons.OK, MessageBoxIcon.None);
                

                //导出excel
                myUtils.ExportExcel(this.successEmails, Application.StartupPath + SysConstant.FILE_EXCEL_TEMPLATE, this.config.SenderEmail);

                //把本地已经发送过得邮件 excel删除掉,打包之后还是会报错。。。。
                foreach (Email mail in this.successEmails)
                {
                    string filePath = sendFilePath + mail.recipientEmail + "/";
                    if (Directory.Exists(filePath))
                    {
                        myUtils.deleteDirectory(filePath);
                        //垃圾回收
                        GC.Collect();
                    }
                }

                //跟新 localEmails和myUtils.objCache["Emails"] 的数据为 发送失败的数据
                //this.localEmails = this.failedEmails;
                myUtils.addOrUpdateCache(SysConstant.CACHE_EMAILS, this.failedEmails);

                //清空XML文件  EmailRoot 节点 下的所有子节点
                myUtils.clearXMLNode(this.emailFilePath, "EmailRoot");

                //跟新Emails.xml文件
                foreach (Email mail in this.failedEmails)
                {
                    myUtils.XMLWriterByEmail(mail,this.emailFileFolder,this.emailFilePath);
                }

                if(this.threadSendEmail != null && this.threadSendEmail.IsAlive){
                    this.threadSendEmail.Abort();
                    //结束线程后 修改缓存中线程状态
                    myUtils.addOrUpdateCache(SysConstant.CACHE_EMAIL_SEBD_STATE, false);
                }

                
            }
        }

        /// <summary>
        /// 定义一个委托实现回调函数
        /// </summary>
        /// <param name="list"></param>
        public delegate void CallBackDelegate(int a,int b,int c);
        #endregion












    }
}
