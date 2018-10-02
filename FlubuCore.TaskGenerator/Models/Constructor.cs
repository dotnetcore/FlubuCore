using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.TaskGenerator.Models
{
    public class Constructor
    {
        public string Summary { get; set; }

        public List<Parameter> ConstructorParameters { get; set; }
    }
}
