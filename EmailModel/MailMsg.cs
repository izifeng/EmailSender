using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class MailMsg
    {
        /// <summary>
        /// 邮件发送者 邮箱号
        /// </summary>
        public string senderID{ get;set;}

        /// <summary>
        /// 邮件发送者 邮箱密码
        /// </summary>
        public string senderPwd { get; set; }

        /// <summary>
        /// 邮件发送人名称
        /// </summary>
        public string senderName { get; set; }

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string emailTitle { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string emailContent { get; set; }

        /// <summary>
        /// 收件人 邮箱号
        /// </summary>
        public string recipientEmail { get; set; }

        /// <summary>
        /// 收件人 名称
        /// </summary>
        public string recipientName { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<string> attachments { get; set; }
    }
}
