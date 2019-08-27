using System.Collections.Generic;

namespace FlubuCore.TaskGenerator.Models
{
    public class Task
    {
        public string FileName { get; set; }

        public string TaskName { get; set; }

        public string TaskDescription { get; set; }

        public string TaskResult { get; set; }

        public string Parent { get; set; }

        public string ProjectName { get; set; }

        public string ExecutablePath { get; set; }

        public string Namespace { get; set; }

        public Constructor Constructor { get; set; }

        public List<Method> Methods { get; set; }
     }
}
