using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Models;
using System.IO;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using System.Drawing;
using System.Xml;
using System.Collections;
using System.Net.Mime;
using System.Data;

namespace Utils
{
    public class myUtils
    {
        /// <summary>
        /// 用来缓存 软件配置信息
        /// </summary>
        public static Hashtable objCache = new Hashtable();


        #region 验证是否正确的Email地址 杨大鑫
        /// <summary>
        /// 验证是否正确的Email地址
        /// </summary>
        public static bool IsEmail(string value)
        {
            return Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        #endregion

        #region 邮件发送 杨大鑫
        /// <summary>
        /// 邮件发送 杨大鑫
        /// </summary>
        /// <param name="email">邮件对象</param>
        /// <returns></returns>
        public static bool sendEmail(MailMsg email) 
        {
            String host = SysConstant.MAIL_SMTP_HOST_LM;//指定 smtp 服务器地址
            int port = SysConstant.MAIL_SMTP_PORT_LM;//指定 smtp 服务器的端口，默认是25，如果采用默认端口，可省去

            Int32 modelIndex = (Int32)myUtils.getCache(SysConstant.CACHE_MODEL_INDEX);//模式，0=朗玛信息，1=39健康网
            if (modelIndex == 1)
            {
                host = SysConstant.MAIL_SMTP_HOST_39;
                port = SysConstant.MAIL_SMTP_PORT_39;
            }

            String senderID = email.senderID;//邮件发送人 邮箱账号
            String senderPwd = email.senderPwd;//邮件发送人 邮箱密码

            SmtpClient smtp = new SmtpClient(); //实例化一个SmtpClient
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; //将smtp的出站方式设为 Network
            smtp.EnableSsl = false;//smtp服务器是否启用SSL加密

            smtp.Host = host; //指定 smtp 服务器地址
            smtp.Port = port; //指定 smtp 服务器的端口，默认是25，如果采用默认端口，可省去
            smtp.Timeout = 30000;

            //如果需要认证，则用下面的方式
            smtp.Credentials = new NetworkCredential(senderID, senderPwd);

            MailMessage mm = new MailMessage(); //实例化一个邮件类
            mm.Priority = MailPriority.High; //邮件的优先级，分为 Low, Normal, High，通常用 Normal即可

            //收件方看到的邮件来源；
            //第一个参数是发信人邮件地址
            //第二参数是发信人显示的名称
            //第三个参数是 第二个参数所使用的编码，如果指定不正确，则对方收到后显示乱码
            mm.From = new MailAddress(senderID, email.senderName, Encoding.GetEncoding(936));

            mm.To.Add(email.recipientEmail);//邮件的接收者，支持群发，多个地址之间用 半角逗号 分开

            mm.Subject = email.emailTitle; //邮件标题
            mm.SubjectEncoding = Encoding.GetEncoding(936);// 这里非常重要，如果你的邮件标题包含中文，这里一定要指定，否则对方收到的极有可能是乱码。 936是简体中文的pagecode，如果是英文标题，这句可以忽略不用
            mm.IsBodyHtml = true; //邮件正文是否是HTML格式

            mm.BodyEncoding = Encoding.GetEncoding(936);//邮件正文的编码， 设置不正确， 接收者会收到乱码

            mm.Body = email.emailContent;//邮件正文

            //添加附件
            string mailCallPrefix = SysConstant.MAIL_CALL_PREFIX;
            Attachment att = null;
            List<Attachment> attList = new List<Attachment>();
            foreach (string filePath in email.attachments) {
                if (File.Exists(filePath))
                {
                    att = new Attachment(filePath);
                    att.Name = mailCallPrefix +" "+ email.recipientName + SysConstant.FILE_EXCEL_SUFFIX;
                    mm.Attachments.Add(att);
                    attList.Add(att);
                    att.ContentStream.Flush();
                }
            }

            bool flag = true;
            try
            {
                smtp.Send(mm); //发送邮件，如果不返回异常， 则大功告成了。
            }
            catch(Exception e)
            {
                //throw new Exception(e.Message);
                flag = false;
            }

            //释放流
            foreach (Attachment atts in attList) {
                if (atts.ContentStream != null) {
                    atts.ContentStream.Dispose();
                }
            }
            
            //垃圾回收
            GC.Collect();
            return flag;
        }
        #endregion

        #region 读取Excel文件 杨大鑫
        /// <summary>
        /// 提取excel数据
        /// </summary>
        /// <param name="fileNames">文件名称数组</param>
        public static List<Email> extractExcelData(List<string> fileNames, string saveFilePath)
        {

            List<Email> list = (List<Email>)myUtils.getCache(SysConstant.CACHE_EMAILS);
            List<Email> emails = new List<Email>();

            Int32 modelIndex = (Int32)myUtils.getCache(SysConstant.CACHE_MODEL_INDEX);//模式，0=朗玛信息，1=39健康网

            int senderNum = 1;
            if (list != null && list.Count > 0)
            {
                senderNum = list.Count + 1;
                emails.AddRange(list);
            }

            try
            {
                //检测文件夹是否存在
                if (!Directory.Exists(saveFilePath))
                {
                    Directory.CreateDirectory(saveFilePath);
                }

                
                for (int i = 0; i < fileNames.Count; i++)
                {
                    Workbook workbook = new Workbook(fileNames[i]);
                    WorksheetCollection wsc = workbook.Worksheets; //工作表 
                    int sheetCount = wsc.Count;
                    for (int s = 0; s < sheetCount; s++)
                    {
                        Worksheet sheet = workbook.Worksheets[s]; //工作表 

                        if (sheet.Name.IndexOf("&") > 0)
                        {
                            string[] sArray = sheet.Name.Split('&');
                            string recipientName = sArray[0];
                            string mailSuffix = SysConstant.MAIL_SUFFIX_LM;
                            if (modelIndex == 1)
                            {
                                mailSuffix = SysConstant.MAIL_SUFFIX_39;
                            }

                            string recipientEmail = sArray[1] + mailSuffix;

                            Cells cells = sheet.Cells;
                            string emailTitle = cells[0].Value.ToString().Trim();

                            //另存excel
                            string filePath = saveFilePath + recipientEmail + "/";
                            myUtils.SaveFile(sheet, recipientName, filePath);


                            //把从Excel中提取的信息写到本地XML文件中，以方便后面的操作
                            Email email = new Email();
                            email.id = senderNum;
                            email.recipientName = recipientName;
                            email.recipientEmail = recipientEmail;
                            email.emailTitle = emailTitle;
                            email.state = "未发送";
                            email.date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            //判断是否 已经添加过相同的，相同的就不添加了
                            bool b = false;
                            foreach (Email m in emails)
                            {
                                if (m.recipientEmail.Equals(recipientEmail)) {
                                    b = true;
                                    break;
                                }
                            }

                            if(!b)
                            {
                                emails.Add(email);
                                senderNum++;
                            }

                            

                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                emails = null;
            }
            return emails;
        }
        #endregion


        #region 把worksheet 另存为excel 杨大鑫
        public static void SaveFile(Worksheet sheet, string sheetName, string filePath)
        {
            using (MemoryStream stream = OutFileToStream(sheet, sheetName))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string file = filePath + sheetName + SysConstant.FILE_EXCEL_SUFFIX;

                if (File.Exists(file))
                {
                    string newName = sheetName + "_"+DateTime.Now.ToString("yyyyMMddHHmmss");
                    file = filePath + newName + SysConstant.FILE_EXCEL_SUFFIX;
                }

                using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write))
                {
                    
                    byte[] data = stream.ToArray();
                    fs.Lock(0, data.Length);//给文件流加锁
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    fs.Dispose();
                }
            }
        }


        public static MemoryStream OutFileToStream(Worksheet ws, string sheetName)
        {
            Workbook workbook = new Workbook(); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            sheet.Copy(ws);
            sheet.Name = sheetName;
            MemoryStream ms = workbook.SaveToStream();
            return ms;
        }

        /// <summary>
        /// excel导出
        /// </summary>
        /// <param name="list"></param>
        /// <param name="excelMb">excel模板路径</param>
        public static void ExportExcel(List<Email> list, string excelMb, string senderEmail)
        {
            DataTable dt = myUtils.ArrayToDataTable(list);

            dt.TableName = "tb";

            WorkbookDesigner designer = new WorkbookDesigner();

            if (!File.Exists(excelMb)) {
                return;
            }
            designer.Open(excelMb);

            //数据源 
            designer.SetDataSource(dt);

            designer.SetDataSource("SenderEmail", senderEmail);

            designer.SetDataSource("SenderDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            designer.Process();

            //文件名称
            String fileName = SysConstant.APP_NAME ;
            string file = myUtils.getCache(SysConstant.CACHE_DESKPATH.ToString()) + "/" + fileName + SysConstant.FILE_EXCEL_SUFFIX;

            MemoryStream stream = designer.Workbook.SaveToStream();

            if (File.Exists(file))
            {
                string newName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                file = myUtils.getCache(SysConstant.CACHE_DESKPATH.ToString()) + "/" + newName + SysConstant.FILE_EXCEL_SUFFIX;
            }

            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write))
            {

                byte[] data = stream.ToArray();
                fs.Lock(0, data.Length);//给文件流加锁
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Dispose();
            }
        }

        #endregion


        #region XML 文件读写 杨大鑫
        /// <summary>    
        /// 创建节点    
        /// </summary>    
        /// <param name="xmldoc"></param>  xml文档  
        /// <param name="parentnode"></param>父节点    
        /// <param name="name"></param>  节点名  
        /// <param name="value"></param>  节点值  
        ///   
        public static void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        /// <summary>
        /// 写Emails.xml文件
        /// </summary>
        public static void XMLWriterByEmail(Email email, string emailFileFolder, string emailFilePath)
        {

            if (!Directory.Exists(emailFileFolder))
            {
                Directory.CreateDirectory(emailFileFolder);
            }

            XmlDocument xmlDoc = new XmlDocument();

            //根节点
            XmlNode root = null;

            //判断文件是否存在
            if (!File.Exists(emailFilePath))
            {
                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);

                //创建 根节点
                root = xmlDoc.CreateElement("EmailRoot");
                xmlDoc.AppendChild(root);
            }
            else
            {
                xmlDoc.Load(emailFilePath);

                root = xmlDoc.SelectSingleNode("EmailRoot");//获取根节点
            }

            XmlNode emailNode = xmlDoc.CreateNode(XmlNodeType.Element, "Email", null);
            myUtils.CreateNode(xmlDoc, emailNode, "Id", email.id.ToString());
            myUtils.CreateNode(xmlDoc, emailNode, "RecipientName", email.recipientName);
            myUtils.CreateNode(xmlDoc, emailNode, "RecipientEmail", email.recipientEmail);
            myUtils.CreateNode(xmlDoc, emailNode, "EmailTitle", email.emailTitle);
            myUtils.CreateNode(xmlDoc, emailNode, "State", email.state.ToString());
            myUtils.CreateNode(xmlDoc, emailNode, "Date", email.date.ToString());
            root.AppendChild(emailNode);

            try
            {
                xmlDoc.Save(emailFilePath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }


        /// <summary>
        /// 读取Emails.xml文件
        /// </summary>
        /// <returns></returns>
        public static List<Email> XMLReaderByEmail(String emailFilePath)
        {
            List<Email> emails = new List<Email>();

            if (!File.Exists(emailFilePath))
            {
                return emails;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(emailFilePath);

            //查找EmailContent节点
            XmlNode root = xmlDoc.SelectSingleNode("EmailRoot");

            //获得根节点的所有子节点
            XmlNodeList nodes = root.ChildNodes;

            foreach (XmlNode node in nodes)
            {
                Email email = new Email();
                XmlNodeList xnl = node.ChildNodes;

                //Id
                XmlNode id = node.SelectSingleNode("Id");
                email.id = id.FirstChild != null ? System.Int32.Parse(id.FirstChild.Value.ToString()) : 0;

                //RecipientName
                XmlNode recipientName = node.SelectSingleNode("RecipientName");
                email.recipientName = recipientName.FirstChild != null ? recipientName.FirstChild.Value.ToString() : "";

                //RecipientEmail
                XmlNode recipientEmail = node.SelectSingleNode("RecipientEmail");
                email.recipientEmail = recipientEmail.FirstChild != null ? recipientEmail.FirstChild.Value.ToString() : "";

                //EmailTitle
                XmlNode emailTitle = node.SelectSingleNode("EmailTitle");
                email.emailTitle = emailTitle.FirstChild != null ? emailTitle.FirstChild.Value.ToString() : "";

                //State
                XmlNode state = node.SelectSingleNode("State");
                email.state = state.FirstChild != null ? state.FirstChild.Value.ToString() : "";

                //Date
                XmlNode date = node.SelectSingleNode("Date");
                email.date = date.FirstChild != null ? date.FirstChild.Value.ToString() : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                emails.Add(email);
            }
            return emails;
        }

        /// <summary>
        /// 写入配置文件 Config.xml
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static bool XMLWriterByConfig(Config cfg, string startupPath)
        {
            bool flag = true;
            string configFilePath = startupPath + SysConstant.FILE_CONFIG;
            try
            {
                if (!Directory.Exists(startupPath + SysConstant.FOLDER_FILE))
                {
                    Directory.CreateDirectory(startupPath + SysConstant.FOLDER_FILE);
                }

                XmlDocument xmlDoc = new XmlDocument();


                //判断文件是否存在，不存在 则为 新增，存在则为 修改
                if (!File.Exists(configFilePath))
                {
                    //创建类型声明节点  
                    XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                    xmlDoc.AppendChild(node);

                    //创建 根节点
                    XmlNode root = xmlDoc.CreateElement("ConfigRoot");
                    xmlDoc.AppendChild(root);

                    XmlNode configNode = xmlDoc.CreateNode(XmlNodeType.Element, "Config", null);
                    myUtils.CreateNode(xmlDoc, configNode, "SenderEmail", cfg.SenderEmail);
                    myUtils.CreateNode(xmlDoc, configNode, "EmailPwd", cfg.EmailPwd);
                    myUtils.CreateNode(xmlDoc, configNode, "IsSaveEmail", cfg.IsSaveEmail.ToString());
                    root.AppendChild(configNode);

                    xmlDoc.Save(configFilePath);
                }
                else
                {
                    //修改
                    xmlDoc.Load(configFilePath);

                    XmlNode root = xmlDoc.SelectSingleNode("ConfigRoot");//获取根节点

                    //获取Config 节点
                    XmlNode configNode = root.SelectSingleNode("Config");

                    //SenderEmail
                    XmlNode SenderEmail = configNode.SelectSingleNode("SenderEmail");
                    SenderEmail.InnerText = cfg.SenderEmail;

                    //RecipientName
                    XmlNode EmailPwd = configNode.SelectSingleNode("EmailPwd");
                    EmailPwd.InnerText = cfg.EmailPwd;

                    //IsSaveEmail
                    XmlNode IsSaveEmail = configNode.SelectSingleNode("IsSaveEmail");
                    IsSaveEmail.InnerText = cfg.IsSaveEmail.ToString();

                    xmlDoc.Save(configFilePath);
                }

            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 清空XML文件  rootName 节点 下的所有子节点
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="rootName">根节点名称</param>
        public static void clearXMLNode(string filePath,string rootName) {
            //判断文件是否存在
            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(filePath);

                //查找EmailContent节点
                XmlNode root = xmlDoc.SelectSingleNode(rootName);

                XmlNodeList xnl = root.ChildNodes;
                if (xnl.Count > 0)
                {
                    root.RemoveAll();
                    xmlDoc.Save(filePath);
                }
            }
        }

        /// <summary>
        /// 读取Config.xml文件
        /// </summary>
        /// <returns></returns>
        public static Config XMLReaderByConfig(string configFilePath)
        {
            Config cfg = new Config();

            if (!File.Exists(configFilePath))
            {
                return null;
            }

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(configFilePath);

            //查找EmailContent节点
            XmlNode root = xmlDoc.SelectSingleNode("ConfigRoot");

            //获取Config 节点
            XmlNode configNode = root.SelectSingleNode("Config");

            //SenderEmail
            XmlNode SenderEmail = configNode.SelectSingleNode("SenderEmail");
            cfg.SenderEmail = SenderEmail.FirstChild != null ? SenderEmail.FirstChild.Value.ToString() : "";

            //RecipientName
            XmlNode EmailPwd = configNode.SelectSingleNode("EmailPwd");
            cfg.EmailPwd = EmailPwd.FirstChild != null ? EmailPwd.FirstChild.Value.ToString() : "";

            //IsSaveEmail
            XmlNode IsSaveEmail = configNode.SelectSingleNode("IsSaveEmail");
            cfg.IsSaveEmail = IsSaveEmail.FirstChild != null ? System.Boolean.Parse(IsSaveEmail.FirstChild.Value.ToString()) : false;

            return cfg;
        }

        #endregion

        #region
        /// <summary>
        /// 添加或者修改缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value"></param>
        public static void addOrUpdateCache(string key, Object value)
        {
            if (myUtils.objCache.Contains(key))
            {
                myUtils.objCache.Remove(key);
                myUtils.objCache.Add(key, value);
            }
            else
            {
                myUtils.objCache.Add(key, value);
            }
        }

        /// <summary>
        /// 获取缓存键值
        /// </summary>
        /// <param name="key"></param>
        public static Object getCache(string key)
        {
            if (myUtils.objCache.Contains(key))
            {
                return myUtils.objCache[key];
            }
            else {
                return null;
            }
            
        }

        public static bool existsCache(string key) {
            if (myUtils.objCache.Contains(key))
            {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// 删除缓存键
        /// </summary>
        public static void deleteCache(string key) {
            if (myUtils.objCache.Contains(key))
            {
                myUtils.objCache.Remove(key);
            } 
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public static void clearCache()
        {
            myUtils.objCache.Clear();
        }
        #endregion


        /// <summary>
        /// 删除非空文件夹
        /// </summary>
        /// <param name="path">要删除的文件夹目录</param>
        public static void deleteDirectory(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    DirectoryInfo[] childs = dir.GetDirectories();
                    foreach (DirectoryInfo child in childs)
                    {
                        child.Delete(true);
                    }
                    dir.Delete(true);
                }
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
            
        }

        #region
        /// <summary>
        /// 数组转DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ArrayToDataTable(List<Email> list) {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("recipientName");
            dt.Columns.Add("recipientEmail");
            dt.Columns.Add("emailTitle");
            dt.Columns.Add("emailContent");
            dt.Columns.Add("state");
            dt.Columns.Add("date");

            foreach (Email email in list)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = email.id;
                dr["recipientName"] = email.recipientName;
                dr["recipientEmail"] = email.recipientEmail;
                dr["emailTitle"] = email.emailTitle;
                dr["emailContent"] = email.emailContent;
                dr["state"] = email.state;
                dr["date"] = email.date;
                dt.Rows.Add(dr);
                dt.AcceptChanges();
            }

            return dt;
        }
        #endregion


    }


}
