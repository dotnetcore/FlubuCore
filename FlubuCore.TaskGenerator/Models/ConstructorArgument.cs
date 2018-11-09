using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.TaskGenerator.Models
{
    public class ConstructorArgument : Argument
    {
        /// <summary>
        /// Denominates if argument(command) is optional or not.
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// If <c>true</c> argument(command) is added after all options. Otherwise before all options.
        /// </summary>
        public bool AfterOptions { get; set; }
    }
}
