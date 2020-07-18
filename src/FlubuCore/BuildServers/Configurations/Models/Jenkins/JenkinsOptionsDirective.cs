using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public class JenkinsOptionsDirective
    {
        public int? BuildDiscarder { get; set; }

        public string CheckoutToSubDirectory { get; set; }

        public bool DisableConcurrentBuilds { get; set; }

        public bool DisableResume { get; set; }

        public int? PreserveStashes { get; set; }

        public int? QuietPeriod { get; set; }

        public int? Retry { get; set; }

        public bool SkipDefaultCheckout { get; set; }

        public bool SkipStagesAfterUnstable { get; set; }

        public int? Timeout { get; set; }

        public string TimeoutUnit { get; set; }

        public bool TimeStamps { get; set; }
    }
}
