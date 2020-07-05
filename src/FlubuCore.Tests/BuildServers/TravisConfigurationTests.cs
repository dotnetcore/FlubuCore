using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.BuildServers.Configurations;
using FlubuCore.BuildServers.Configurations.Models;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines;
using FlubuCore.BuildServers.Configurations.Models.AzurePipelines.Job;
using FlubuCore.BuildServers.Configurations.Models.Travis;
using Xunit;

namespace FlubuCore.Tests.BuildServers
{
    public class TravisConfigurationTests
    {
        [Fact(Skip = "")]
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

        [Fact(Skip = "")]
        public void Test2()
        {
            YamlConfigurationSerializer serializer = new YamlConfigurationSerializer();
            var azure = new AzurePipelinesConfiguration
            {
                Jobs = new List<JobItem>()
                {
                    new JobItem()
                    {
                        Job = "YYY",
                    },
                    new JobItem() { Job = "Abc" }
                }
            };

            azure.Jobs[0].AddStep(new TaskItem() { Task = "Task", DisplayName = "Display name" });
            azure.Jobs[0].AddStep(new ScriptItem() { Script = "Start", DisplayName = "SU" });

            azure.Jobs[1].AddStep(new TaskItem() { Task = "Task2", DisplayName = "Display name2" });
            azure.Jobs[1].AddStep(new ScriptItem() { Script = "Start2", DisplayName = "SU2" });
            var yaml = serializer.Serialize(azure);
            File.WriteAllText("D:/test.yaml", yaml);
        }
    }
}
