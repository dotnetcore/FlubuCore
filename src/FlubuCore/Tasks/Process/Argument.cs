using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Process
{
    internal class Argument
    {
        public Argument(string argKey, string argValue, bool valueRequired, bool maskArg, string separator = null)
        {
            ArgKey = argKey;
            ArgValue = argValue;
            ValueRequired = valueRequired;
            MaskArg = maskArg;
            Separator = separator;
        }

        public string ArgKey { get; set; }

        public string ArgValue { get; set; }

        /// <summary>
        /// If <c>true</c> exception is thrown if value is not present, otherwise not.
        /// </summary>
        public bool ValueRequired { get; set; }

        /// <summary>
        /// If <c><true</c> argument is masked when logging to console, otherwise not.
        /// </summary>
        public bool MaskArg { get; set; }

        /// <summary>
        /// Key value separator.
        /// </summary>
        public string Separator { get; set; }
    }
}
