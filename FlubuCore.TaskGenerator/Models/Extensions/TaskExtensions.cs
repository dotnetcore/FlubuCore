using System.Collections.Generic;

namespace FlubuCore.TaskGenerator.Models.Extensions
{
    public class TaskExtensions
    {
        public string FileName { get; set; }

        public string ExtensionName { get; set; }

        public string Namespace { get; set; }

        public List<string> Usings { get; set; } = new List<string>();

        public List<ExtensionMethod> Methods { get; set; } = new List<ExtensionMethod>();

        public List<TaskExtensions> SubExtensions { get; set; } = new List<TaskExtensions>();
    }

}
