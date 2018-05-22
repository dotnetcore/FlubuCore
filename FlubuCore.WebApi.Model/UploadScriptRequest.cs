using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.WebApi.Model
{
    public class UploadScriptRequest
    {
        /// <summary>
        /// Path to the script file to be uploaded.
        /// </summary>
        public string FilePath { get; set; }

        public override string ToString()
        {
            return $"FilePath '{FilePath}'";
        }
    }
}
