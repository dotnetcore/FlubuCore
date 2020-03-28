using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context.Attributes.BuildProperties
{
    public class OutputDirAttribute : BuildPropertyAttribute
    {
        public OutputDirAttribute()
            : base(BuildProps.BuildDir)
        {
        }
    }
}
