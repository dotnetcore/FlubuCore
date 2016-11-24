using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Context
{
    public class BuildPropertiesContext : IBuildPropertiesContext
    {
        public BuildPropertiesContext(IBuildPropertiesSession properties)
        {
            Properties = properties;
        }

        public IBuildPropertiesSession Properties { get; }
    }
}
