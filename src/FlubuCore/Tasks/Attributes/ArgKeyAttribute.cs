using System;

namespace FlubuCore.Tasks.Attributes
{
    public class ArgKeyAttribute : Attribute
    {
        public ArgKeyAttribute(params string[] keys)
        {
            Keys = keys;
        }

        public string[] Keys { get; set; }
    }
}
