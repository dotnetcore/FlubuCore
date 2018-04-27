using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Configuration
{
    public class WebApiSettings
    {
        public bool AllowScriptUpload { get; set; }

        public List<string> AllowedIps { get; set; }

        public List<TimeFrame> TimeFrames { get; set; }

        public int MaxFailedLoginAttempts { get; set; }

        public bool SecurityNotificationsEnabled { get; set; }

        public List<NotificationFilter> NotificationFilters { get; set; }

        public bool IncludeStackTrace { get; set; }

        public bool AddFlubuLogsToResponse { get; set; }
    }
}
