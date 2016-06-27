using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Email
    {
        /// <summary>
        /// ID
        /// </summary>
        public Int32? id { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public String recipientName { get; set; }

        /// <summary>
        /// 收件人 Email地址
        /// </summary>
        public String recipientEmail { get; set; }

        /// <summary>
        /// Email标题
        /// </summary>
        public String emailTitle { get; set; }

        public String emailContent { get; set; }

        /// <summary>
        /// Email状态 未发送,已发送
        /// </summary>
        public String state { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string date { get; set; }

    }
}
