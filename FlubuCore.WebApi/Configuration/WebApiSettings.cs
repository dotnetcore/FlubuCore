using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Configuration
{
    public class WebApiSettings
    {
		public string SecretKey { get; set; }

		public List<string> AllowedIps { get; set; }

		public List<TimeFrame> TimeFrames { get; set; }
    }
}
