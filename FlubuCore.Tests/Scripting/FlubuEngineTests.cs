using System.Collections.Generic;
using System.IO;
using FlubuCore.Scripting;
using FlubuCore.Tests.Integration;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class BuildScriptEngineTests
    {
        private IFlubuEngine _flubuEngine;

        public BuildScriptEngineTests()
        {
            _flubuEngine = new FlubuEngine();
        }

        [Fact]
        public void BuildScriptEngine_RunSimpleScript_Succesfull()
        {
            var taskSession = _flubuEngine.CreateTaskSession(new BuildScriptArguments
            {
                MainCommands = new List<string>() { "Do" },
            });

            Assert.NotNull(_flubuEngine.ServiceProvider);
            Assert.NotNull(_flubuEngine.TaskFactory);
            Assert.NotNull(_flubuEngine.LoggerFactory);

            SimpleBuildScript script = new SimpleBuildScript();

            var result = script.Run(taskSession);
            Assert.Equal(0, result);
        }

        [Fact]
        public void FlubuEngine_CreateSimpleScriptAndRunIt_Succesfull()
        {
            var path = "TestDirectory".ExpandToExecutingPath();
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
            }

            var taskSession = _flubuEngine.CreateTaskSession(new BuildScriptArguments());
            taskSession.Tasks().CreateDirectoryTask(path, true).Execute(taskSession);
            Assert.True(Directory.Exists(path));
        }

        [Fact]
        public void CreateHttpClientInTaskTest()
        {
            var taskSession = _flubuEngine.CreateTaskSession(new BuildScriptArguments
            {
                MainCommands = new List<string>() { "HttpClient" },
            });

            BuildScriptWithHttpClient script = new BuildScriptWithHttpClient();

            var result = script.Run(taskSession);
            Assert.Equal(0, result);
        }
    }
}
