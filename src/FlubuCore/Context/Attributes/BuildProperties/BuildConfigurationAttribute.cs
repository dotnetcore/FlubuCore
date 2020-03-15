using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    /// <summary>
    ///  Get or sets BuildConfiguration  from build properties session.
    /// </summary>
    public class BuildConfigurationAttribute : BuildPropertyAttribute
    {
        public BuildConfigurationAttribute()
            : base(BuildProps.BuildConfiguration)
        {
        }
    }
}
