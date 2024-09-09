using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class EmailConfiguration : IEmailConfiguration
    {
        public string SmtpServer { get; set; }

        public string SmtpUsername { get; set; }

        public int SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string DisplayName { get; set; }
    }
    public interface IEmailConfiguration
    {
        string SmtpServer { get; }
        int SmtpPort { get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }
        string DisplayName { get; set; }
    }
}
