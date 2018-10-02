using System.Collections.Generic;

namespace FlubuCore.TaskGenerator.Models
{
    public class Task
    {
        public string FileName { get; set; }

        public string TaskName { get; set; }

        public string ProjectName { get; set; }

        // ReSharper disable once InconsistentNaming
        public string TResult { get; set; } = "int";

        public Constructor Constructor { get; set; }

        public List<Method> Methods { get; set; }
     }
}
