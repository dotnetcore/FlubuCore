using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Configuration
{
    public class NotificationSettings
    {
        public string EmailFrom { get; set; }

        public List<string> EmailTo { get; set; }

        public string SmtpServerHost { get; set; }

        public int SmtpServerPort { get; set; }
    }
}
