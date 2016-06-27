using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Models;
using System.Threading;
using System.Xml;

namespace MailSender
{
    public partial class main : Form
    {
        /// <summary>
        /// 用来存放 程序中 所创建的线程
        /// </summary>
        private List<Thread> threadList = new List<Thread>();

        /// <summary>
        /// 存放当前grid中邮件信息
        /// </summary>
        private List<Email> localEmails = new List<Email>();

        private BindingSource bs = new BindingSource();

        private string saveFilePath = Application.StartupPath + SysConstant.FOLDER_SEND;

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
        /// 文件选择框
        /// </summary>
        private string[] fileNames = null;

        /// <summary>
        /// 文件夹选择框
        /// </summary>
        private string folderFullName = "";


        public main()
        {
            InitializeComponent();
            readEmailByXML();


            //创建一个线程，用来实现时钟
            ClockCallBackDelegate cbc = clockCallBack;
            Thread threadClock = new Thread(new ParameterizedThreadStart(clockRun));
            threadClock.IsBackground = true;
            threadClock.Name = "clock";
            threadClock.Start(cbc);
            this.threadList.Add(threadClock);
        }

        #region 时钟
        /// <summary>
        /// 时钟
        /// </summary>
        /// <param name="time"></param>
        private delegate void ClockCallBackDelegate(string time);

        private void clockCallBack(string time)
        {
            this.toolStripStatusLabel2.Text = time;
        }

        private void clockRun(Object o)
        {
            while (true)
            {
                String now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ClockCallBackDelegate cbc = o as ClockCallBackDelegate;
                cbc(now);
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region 文件选择框
        /// <summary>
        /// 文件选择框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Title = "选择文件";
            dialog.Filter = "*.xls|*.xls|*.xlsx|*.xlsx";
            DialogResult dr = dialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                DialogResult result = MessageBoxEx.Show(this, "确定要导入该数据吗？导入数据可能会花一段时间，请耐心等待!", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    this.fileNames = dialog.FileNames;

                    //新开一个线程，用来操作文件的读取
                    FileCallBackDelegate cbc = FileCallBack;
                    Thread threadFile = new Thread(new ParameterizedThreadStart(fileRun));
                    threadFile.IsBackground = true;
                    threadFile.Name = "file";
                    threadFile.Start(cbc);
                    this.threadList.Add(threadFile);
                }

            }

        }

        private void fileRun(Object o)
        {
            string[] fileNames = this.fileNames;

            List<string> newFileNames = new List<string>(fileNames.Length);

            for (int i = 0; i < fileNames.Length; i++)
            {
                string fileName = fileNames[i].Substring(fileNames[i].LastIndexOf("\\") + 1);
                string fileNameExt = fileName.Substring(fileName.LastIndexOf(".") + 1);

                string filter = "xls,xlsx";
                if (!filter.Contains(fileNameExt))//文件类型不满足条件
                {
                    MessageBoxEx.Show(this, "文件格式不正确!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
                else
                {
                    newFileNames.Add(fileNames[i]);
                }
            }
            //提取excel数据,并把数据绑定到grid中

            this.localEmails = myUtils.extractExcelData(newFileNames, saveFilePath);
            

            //把邮件信息缓存起来，方便发送的时候使用
            myUtils.addOrUpdateCache(SysConstant.CACHE_EMAILS,this.localEmails);

            //执行回调
            FileCallBackDelegate cbd = o as FileCallBackDelegate;
            cbd(this.localEmails);
        }

        private void FileCallBack(List<Email> emails)
        {
            if (this.dataGridView1.InvokeRequired)
            {//不同一个线程
                FileCallBackDelegate fc = new FileCallBackDelegate(FileCallBack);
                Invoke(fc, this.localEmails);//执行唤醒操作
            }
            else
            {//同一个线程
                this.bs.DataSource = this.localEmails;
                this.dataGridView1.DataSource = this.bs;
            }

            //把文件选择框和文件夹选择线程关闭
            for (int i = 0; i < this.threadList.Count; i++)
            {
                if (this.threadList[i].Name.Equals("file") || this.threadList[i].Name.Equals("folder"))
                {
                    if (this.threadList[i].IsAlive)
                    {
                        this.threadList[i].Abort();
                        this.threadList.Remove(this.threadList[i]);
                    }
                }
            }
            //资源回收
            GC.Collect();
        }

        /// <summary>
        /// 定义一个委托实现回调函数
        /// </summary>
        /// <param name="list"></param>
        public delegate void FileCallBackDelegate(List<Email> emails);

        #endregion

        #region 文件夹选择框
        /// <summary>
        /// 文件夹选择框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult dr = dialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                DialogResult result = MessageBoxEx.Show(this, "确定要导入该数据吗？导入数据可能会花一段时间，请耐心等待...", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result == DialogResult.OK)
                {
                    this.folderFullName = dialog.SelectedPath;

                    //新开一个线程，用来操作文件的读取
                    FileCallBackDelegate cbc = FileCallBack;
                    Thread threadFolder = new Thread(new ParameterizedThreadStart(folderRun));
                    threadFolder.IsBackground = true;
                    threadFolder.Name = "folder";
                    threadFolder.Start(cbc);
                    this.threadList.Add(threadFolder);
                }

            }
        }

        private void folderRun(Object o)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(this.folderFullName);

            List<string> listFolder = new List<string>();
            listFolder.Add(this.folderFullName);
            List<string> listFile = new List<string>();

            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                listFolder.Add(NextFolder.FullName);
            }

            for (int i = 0; i < listFolder.Count; i++)
            {
                DirectoryInfo folder = new DirectoryInfo(listFolder[i]);
                //遍历文件
                foreach (FileInfo NextFile in folder.GetFiles())
                {
                    string fileFullName = NextFile.FullName;
                    //只获取excel文件
                    string fileName = fileFullName.Substring(fileFullName.LastIndexOf("\\") + 1);
                    string fileNameExt = fileName.Substring(fileName.LastIndexOf(".") + 1);

                    string filter = "xls,xlsx";
                    if (filter.Contains(fileNameExt))
                    {
                        listFile.Add(fileFullName);
                    }

                }
            }

            if (listFile.Count == 0)
            {
                MessageBoxEx.Show(this, "没有需要导入的文件!", "提示",MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Warning);
                return;
            }

            //提取excel数据,并把数据绑定到grid中
            this.localEmails = myUtils.extractExcelData(listFile, saveFilePath);


            //把邮件信息缓存起来，方便发送的时候使用
            myUtils.addOrUpdateCache(SysConstant.CACHE_EMAILS,this.localEmails);

            //执行回调
            FileCallBackDelegate cbd = o as FileCallBackDelegate;
            cbd(this.localEmails);

        }

        #endregion

        #region 窗体关闭方法
        /// <summary>
        /// 窗体关闭方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_closing(object sender, FormClosingEventArgs e)
        {
            bool flag = false;

            int count = this.localEmails.Count;
            if (count > 0)
            {
                foreach (Email mail in this.localEmails)
                {
                    if (mail != null && !mail.state.Equals("已发送") && !string.IsNullOrWhiteSpace(mail.state))
                    {
                        flag = true;
                        break;
                    }
                }
                //读取配置文件，判断用户是否设置不保留未发送的邮件
                Config cfg = null;
                if (!myUtils.existsCache(SysConstant.CACHE_CONFIG))
                {
                    cfg = myUtils.XMLReaderByConfig(configFilePath);
                }
                else
                {
                    cfg = (Config)myUtils.getCache(SysConstant.CACHE_CONFIG);
                }

                if (cfg != null && cfg.IsSaveEmail && flag)
                {
                    DialogResult result = MessageBoxEx.Show(this, "您有未发送的邮件，确定要退出吗？", "提示", MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
                    if (result == DialogResult.OK)
                    {
                        //判断系统是否正在发送邮件中
                        bool state = false;
                        if (myUtils.existsCache(SysConstant.CACHE_EMAIL_SEBD_STATE))
                        {
                            state = (bool)myUtils.getCache(SysConstant.CACHE_EMAIL_SEBD_STATE);
                        }

                        if (state) {
                            DialogResult dr = MessageBoxEx.Show(this,"正在发送邮件，是否确定退出？","提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Question);
                            if (dr == DialogResult.Cancel) {
                                return;
                            }
                        }

                        //清空XML文件  EmailRoot 节点 下的所有子节点
                        myUtils.clearXMLNode(this.emailFilePath, "EmailRoot");

                        //把未发送的邮件信息存到本地XML中，下次打开软件的时候自动加载到 grid中
                        foreach (Email email in this.localEmails)
                        {
                            if (!email.state.Equals("已发送"))
                            {
                                myUtils.XMLWriterByEmail(email, emailFileFolder, emailFilePath);
                            }
                        }
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }

                    
                }
                //把本地缓存的文件删除掉
                if (cfg == null && Directory.Exists(sendFilePath) || cfg != null && !cfg.IsSaveEmail)
                {
                    myUtils.deleteDirectory(sendFilePath);
                    //垃圾回收
                    GC.Collect();
                }
            }


            //把时钟线程关掉
            //把文件选择框和文件夹选择线程关闭
            foreach (Thread th in this.threadList)
            {
                if (th != null && th.Name.Equals("clock"))
                {
                    if (th.IsAlive)
                    {
                        th.Abort();
                    }
                }
            }

            Application.Exit();
        }
        #endregion

        #region 从本地XML文件中获取 未发送的邮件信息
        /// <summary>
        /// 从本地XML文件中获取 未发送的邮件信息
        /// </summary>
        private void readEmailByXML()
        {
            List<Email> list = myUtils.XMLReaderByEmail(emailFilePath);
            if (list.Count > 0)
            {
                this.bs.DataSource = list;
                this.dataGridView1.DataSource = this.bs;

                this.localEmails = list;

                //把邮件信息缓存起来，方便发送的时候使用
                myUtils.addOrUpdateCache(SysConstant.CACHE_EMAILS,this.localEmails);
            }
            else
            {
                this.localEmails = new List<Email>();
                this.bs.DataSource = this.localEmails;
                this.dataGridView1.DataSource = this.bs;
            }

            //获取系统桌面路径，并存到缓存中
            string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            myUtils.addOrUpdateCache(SysConstant.CACHE_DESKPATH,deskPath);
        }
        #endregion

        #region 系统配置
        /// <summary>
        ///  系统配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            configForm config = new configForm();
            config.ShowDialog();
        }
        #endregion

        #region 发送按钮 实现
        /// <summary>
        /// 发送按钮 实现
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //发送前先判断 是否已经配置过 发送者邮箱和密码
            Config cfg = null;
            if (!myUtils.existsCache(SysConstant.CACHE_CONFIG))
            {
                cfg = myUtils.XMLReaderByConfig(configFilePath);
            }
            else
            {
                cfg = (Config)myUtils.getCache(SysConstant.CACHE_CONFIG);
            }

            if (cfg == null || string.IsNullOrWhiteSpace(cfg.SenderEmail) || string.IsNullOrWhiteSpace(cfg.EmailPwd))
            {
                configForm cf = new configForm();
                cf.ShowDialog();
            }
            else
            {
                contentForm form = new contentForm(this.dataGridView1);
                form.ShowDialog();
            }


        }
        #endregion


        private void readmeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = Application.StartupPath + "/" + "readme.txt";

            System.Diagnostics.Process.Start(filePath);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //判断列索引是不是删除按钮
            if (e.ColumnIndex == this.dataGridView1.Columns["ColumnDelete"].Index)
            {
                
                int rows = dataGridView1.CurrentRow.Index; //得到当前行的索引
                DataGridViewRow row = this.dataGridView1.Rows[rows];
                if (row.DataBoundItem == null) {
                    return;
                }
                string idStr = this.dataGridView1.Rows[rows].Cells[0].Value.ToString(); //获得主键id，就是上表中的编号
                string email = this.dataGridView1.Rows[rows].Cells[2].Value.ToString();
                int id = System.Int32.Parse(idStr);
                if (!string.IsNullOrWhiteSpace(idStr) && MessageBoxEx.Show(this, "您确定要删除吗？", "重要提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    this.dataGridView1.EndEdit();
                    this.dataGridView1.Rows.RemoveAt(rows);
                    for (int i = 0; i < this.localEmails.Count; i++)
                    {
                        if (this.localEmails[i].id.Equals(id))
                        {
                            this.localEmails.Remove(this.localEmails[i]);
                        }
                    }

                    //跟新 localEmails和myUtils.objCache["Emails"] 的数据为 发送失败的数据
                    myUtils.addOrUpdateCache(SysConstant.CACHE_EMAILS, this.localEmails);

                    if (File.Exists(emailFilePath)) {
                        //清空XML文件  EmailRoot 节点 下的所有子节点
                        myUtils.clearXMLNode(this.emailFilePath, "EmailRoot");
                        //跟新Emails.xml文件
                        foreach (Email mail in this.localEmails)
                        {
                            myUtils.XMLWriterByEmail(mail, this.emailFileFolder, this.emailFilePath);
                        }
                    }

                    //把该邮件在本地的文件删除
                    string filePath = sendFilePath + email + "/";
                    if (Directory.Exists(filePath))
                    {
                        myUtils.deleteDirectory(filePath);
                        //垃圾回收
                        GC.Collect();
                    }

                }
            }
        }























    }
}
