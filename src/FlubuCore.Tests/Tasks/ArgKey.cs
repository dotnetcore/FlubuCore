using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tests.Tasks
{
    public class ArgKey : Attribute
    {
        public ArgKey(params string[] keys)
        {
            Keys = keys;
        }

        public string[] Keys { get; set; }
    }
}
