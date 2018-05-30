using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FromArgAttribute : Attribute
    {
        public FromArgAttribute(string argKey, string help = null)
        {
            ArgKey = argKey;
            Help = help;
        }

        public string ArgKey { get; }

        public string Help { get; }
    }
}
