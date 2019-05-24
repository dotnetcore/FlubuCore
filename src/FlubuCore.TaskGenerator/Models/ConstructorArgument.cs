using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.TaskGenerator.Models
{
    public class ConstructorArgument : Argument
    {
        /// <summary>
        /// If <c>true</c> argument(command) is added after all options. Otherwise before all options.
        /// </summary>
        public bool AfterOptions { get; set; }
    }
}
