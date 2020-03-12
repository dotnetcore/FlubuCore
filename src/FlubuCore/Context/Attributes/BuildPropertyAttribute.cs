using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes
{
    public class BuildPropertyAttribute : Attribute
    {
        public BuildPropertyAttribute(string buildProperty)
        {
            BuildProperty = buildProperty;
        }

        public BuildPropertyAttribute(PredefinedBuildProperties buildProperty)
        {
            BuildProperty = buildProperty.ToString();
        }

        public string BuildProperty { get; }
    }
}
