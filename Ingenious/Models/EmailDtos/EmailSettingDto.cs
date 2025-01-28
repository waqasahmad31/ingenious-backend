using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingenious.Models.EmailDtos
{
    public class EmailSettingDto
    {
        public string MailFrom { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SMTPType { get; set; }
        public string Port { get; set; }
    }
}
