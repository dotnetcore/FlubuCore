using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Models
{
    public class ExecuteScript
    {
        public string ScriptName { get; set; }

        public string TargetName { get; set; }
    }
}
