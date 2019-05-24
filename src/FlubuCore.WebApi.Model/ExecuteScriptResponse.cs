using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class ExecuteScriptResponse
    {
        /// <summary>
        /// Flubu web api Server logs.
        /// </summary>
        public List<string> Logs { get; set; }
    }
}
