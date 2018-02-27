using System;
using System.Collections.Generic;
using System.Text;
using Flubu.Tests.Integration;
using FlubuCore.Scripting;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class BuildScriptEngineTests
    {
        private IBuildScriptEngine _buildScriptEngine;

        public BuildScriptEngineTests()
        {
            _buildScriptEngine = new BuildScriptEngine();
        }

        [Fact]
        public void BuildScriptEngine_RunSimpleScript_Succesfull()
        {
           var taskSession = _buildScriptEngine.CreateTaskSession(new BuildScriptArguments
            {
                MainCommands = new List<string>() { "Do" },
                TargetsToExecute = new List<string>() { "Do" }
            });

            Assert.NotNull(_buildScriptEngine.ServiceProvider);
            Assert.NotNull(_buildScriptEngine.TaskFactory);
            Assert.NotNull(_buildScriptEngine.LoggerFactory);

            SimpleBuildScript script = new SimpleBuildScript();

            var result = script.Run(taskSession);
            Assert.Equal(0, result);
        }
    }
}
