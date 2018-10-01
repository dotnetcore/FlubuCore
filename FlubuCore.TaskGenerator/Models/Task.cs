using System.Collections.Generic;

namespace FlubuCore.TaskGenerator.Models
{
    public class Task
    {
        public string FileName { get; set; }

        public string TaskName { get; set; }

        // ReSharper disable once InconsistentNaming
        public string TResult { get; set; } = "int";

        public string ConstuctorSummary { get; set; }

        public List<Parameter> ConstructorParameters { get; set; }

        public List<Method> Methods { get; set; }
     }
}
