using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Process
{
    internal class Argument
    {
        public Argument(string argKey, string argValue, bool valueRequired, bool maskArg)
        {
            ArgKey = argKey;
            ArgValue = argValue;
            ValueRequired = valueRequired;
            MaskArg = maskArg;
        }

        public string ArgKey { get; set; }

        public string ArgValue { get; set; }

        public bool ValueRequired { get; set; }

        public bool MaskArg { get; set; }
    }
}
