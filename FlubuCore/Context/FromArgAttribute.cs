using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    /// <summary>
    /// FromArg is used topass command line arguments, settings from json configuration file or enviroment variables to script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FromArgAttribute : Attribute
    {
        /// <summary>
        /// FromArg is used topass command line arguments, settings from json configuration file or enviroment variables to script.
        /// </summary>
        /// <param name="argKey">The argument key.</param>
        /// <param name="help">Argument help displayed in FlubuCore runner.</param>
        /// <param name="seperator">Value separator when passing lists.</param>
        public FromArgAttribute(string argKey, string help = null, char seperator = ',')
        {
            ArgKey = argKey;
            Help = help;
            Seperator = seperator;
        }

        public string ArgKey { get; }

        public string Help { get; }

        public char Seperator { get; }
    }
}
