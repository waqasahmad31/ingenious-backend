using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingenious.Models.EmailDtos
{
    public class SMTPInfoDto
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string MailFrom { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
