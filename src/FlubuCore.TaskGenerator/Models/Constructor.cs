using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.TaskGenerator.Models
{
    public class Constructor
    {
        public string Summary { get; set; }

        public List<ConstructorArgument> Arguments { get; set; }
    }
}
