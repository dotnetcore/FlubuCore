using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    /// <summary>
    ///  Get or sets BuildDir from build properties session.
    /// </summary>
    public class BuildDirAttribute : BuildPropertyAttribute
    {
        public BuildDirAttribute()
            : base(DotNetBuildProps.BuildDir)
        {
        }
    }
}
