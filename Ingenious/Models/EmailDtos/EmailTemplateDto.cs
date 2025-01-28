using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingenious.Models.EmailDtos
{
    public class EmailTemplateDto
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Template { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
