using System;

namespace FlubuCore.Tasks.Attributes
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
