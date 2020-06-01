using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers.Configurations;
using FlubuCore.BuildServers.Configurations.Models;
using FlubuCore.BuildServers.Configurations.Models.Travis;
using Xunit;

namespace FlubuCore.Tests.BuildServers
{
    public class TravisConfigurationTests
    {
        [Fact]
        public void Test()
        {
            YamlConfigurationSerializer serializer = new YamlConfigurationSerializer();

            var yaml = serializer.Serialize(new TravisConfiguration()
            {
                Script = new List<string>()
                {
                    "export PATH=\"$PATH:$HOME/.dotnet/tools\"",
                    "dotnet tool install --global FlubuCore.GlobalTool --version 5.1.1"
                }
            });
        }
    }
}
