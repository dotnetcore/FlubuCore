using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    /// <summary>
    /// FromArg is used to pass command line arguments, settings from json configuration file or environment variables to script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.GenericParameter, AllowMultiple = true)]
    public class FromArgAttribute : Attribute
    {
        /// <summary>
        /// FromArg is used to pass command line arguments, settings from json configuration file or environment variables to script properties.
        /// </summary>
        /// <param name="argKey">The argument key.</param>
        public FromArgAttribute(string argKey)
        {
            ArgKey = argKey;
        }

        /// <summary>
        /// FromArg is used to pass command line arguments, settings from json configuration file or environment variables to script properties.
        /// </summary>
        /// <param name="argKey">The argument key.</param>
        /// <param name="help">Argument help displayed in FlubuCore tool.</param>
        public FromArgAttribute(string argKey, string help)
        {
            ArgKey = argKey;
            Help = help;
        }

        /// <summary>
        /// FromArg is used to pass command line arguments, settings from json configuration file or environment variables to script properties.
        /// </summary>
        /// <param name="argKey">The argument key.</param>
        /// <param name="help">Argument help displayed in FlubuCore tool.</param>
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
