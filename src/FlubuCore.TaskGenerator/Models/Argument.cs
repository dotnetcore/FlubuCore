using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.TaskGenerator.Models
{
    public class Argument
    {
        public string ArgumentKey { get; set; }

        public string ArgumentKeyPrefix { get; set; } = string.Empty;

        public bool HasArgumentValue { get; set; }

        public Parameter Parameter { get; set; }
    }
}
