using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    /// <summary>
    /// Get or sets solution file name from build properties session.
    /// </summary>
    public class SolutionFileNameAttribute : BuildPropertyAttribute
    {
        public SolutionFileNameAttribute()
            : base(BuildProps.SolutionFileName)
        {
        }
    }
}
