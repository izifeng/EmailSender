using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// 系统常量定义
    /// </summary>
    public class SysConstant
    {
        public static readonly string APP_NAME = "邮件群发器v5.0";

        /// <summary>
        /// 邮件发送状态 缓存键
        /// </summary>
        public static readonly string CACHE_EMAIL_SEBD_STATE = "EmailSendState";

        /// <summary>
        /// 邮件列表
        /// </summary>
        public static readonly string CACHE_EMAILS = "Emails";

        /// <summary>
        /// 系统配置
        /// </summary>
        public static readonly string CACHE_CONFIG = "Config";

        /// <summary>
        /// 模式索引
        /// </summary>
        public static readonly string CACHE_MODEL_INDEX = "model_index";

        /// <summary>
        /// 模式名称
        /// </summary>
        public static readonly string CACHE_MODEL_TEXT = "model_text";

        /// <summary>
        /// 系统桌面路径
        /// </summary>
        public static readonly string CACHE_DESKPATH = "deskPath";

        /// <summary>
        /// 文件默认存放路径
        /// </summary>
        public static readonly string FOLDER_FILE = "/Files/";

        /// <summary>
        /// 待发送文件存放路径
        /// </summary>
        public static readonly string FOLDER_SEND = "/Files/SendFiles/";

        /// <summary>
        /// 存放未发送邮件 的文件 Emails.xml
        /// </summary>
        public static readonly string FILE_EMAILS = "/Files/Emails.xml";

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static readonly string FILE_CONFIG = "/Files/Config.xml";

        /// <summary>
        /// excel模板文件
        /// </summary>
        public static readonly string FILE_EXCEL_TEMPLATE = "/Template.xls";

        /// <summary>
        /// 系统excel文件保存统一格式
        /// </summary>
        public static readonly string FILE_EXCEL_SUFFIX = ".xls";

        /// <summary>
        /// 朗玛邮件后缀名
        /// </summary>
        public static readonly string MAIL_SUFFIX_LM = "@longmaster.com.cn";

        /// <summary>
        /// 39健康网邮件后缀名 mail.39.net
        /// </summary>
        public static readonly string MAIL_SUFFIX_39 = "@mail.39.net";

        /// <summary>
        /// 朗玛邮件smtp服务器地址
        /// </summary>
        public static readonly string MAIL_SMTP_HOST_LM = "mail.longmaster.com.cn";

        /// <summary>
        /// 朗玛邮件stmp服务器端口
        /// </summary>
        public static readonly Int32 MAIL_SMTP_PORT_LM = 25;

        /// <summary>
        /// 39邮件smtp服务器地址
        /// </summary>
        public static readonly string MAIL_SMTP_HOST_39 = "mail.39.net";

        /// <summary>
        /// 39邮件stmp服务器端口
        /// </summary>
        public static readonly Int32 MAIL_SMTP_PORT_39 = 25;

        /// <summary>
        /// 邮件称呼前缀
        /// </summary>
        public static readonly string MAIL_CALL_PREFIX = "Dear";
    }
}
