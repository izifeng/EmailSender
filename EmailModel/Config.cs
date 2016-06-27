using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Config
    {
        /// <summary>
        /// 发送者邮箱地址
        /// </summary>
        public String SenderEmail { get; set; }

        /// <summary>
        /// 邮箱密码
        /// </summary>
        public String EmailPwd { get; set; }

        /// <summary>
        /// 是否保留未发送的 邮件信息
        /// </summary>
        public bool IsSaveEmail { get; set; }
    }
}
