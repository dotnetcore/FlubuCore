using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Services;
using Xunit;

namespace Flubu.Tests.Services
{
    public class FlubuEnviromentTests
    {
        [Fact(Skip = "Explicit test.")]
        
        public void ListAvailableToolsVersionTest()
        {
            var toolsVersion = FlubuEnviroment.ListAvailableMSBuildToolsVersions();
            Assert.NotEqual(0, toolsVersion.Count);
        }
    }
}
