using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Models.ViewModels
{
    public class UpdateCenterModel
    {
        public string LatestVersion { get; set; }

        public string CurrentVersion { get; set; }

        public bool NewVersionExists { get; set; }
    }
}
