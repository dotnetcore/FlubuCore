using System.Collections.Generic;

namespace FlubuCore.TaskGenerator.Models
{
    public class Task
    {
        public string FileName { get; set; }

        public string TaskName { get; set; }

        public string TResult { get; set; } = "int";

        public List<Parameter> ConstructorParameters { get; }

        public List<Method> Methods { get; set; }
     }
}
