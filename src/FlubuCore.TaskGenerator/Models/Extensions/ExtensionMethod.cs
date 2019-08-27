using System.Collections.Generic;

namespace FlubuCore.TaskGenerator.Models.Extensions
{
    public class ExtensionMethod
    {
        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public string MethodName { get; set; }

        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }
}
