using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Flubu.Tests.Scripting
{
    public class ScriptExecutionTests
    {
        private readonly Mock<IFileWrapper> _fileLoader = new Mock<IFileWrapper>();
        private readonly Mock<IDirectoryWrapper> _directory = new Mock<IDirectoryWrapper>();
        private readonly Mock<IScriptAnalyser> _analyser = new Mock<IScriptAnalyser>();
        private readonly Mock<IBuildScriptLocator> _scriptLocator = new Mock<IBuildScriptLocator>();
        private readonly Mock<ILogger<ScriptLoader>> _logger = new Mock<ILogger<ScriptLoader>>();
        private readonly ScriptLoader _loader;

        public ScriptExecutionTests()
        {
            _loader = new ScriptLoader(_fileLoader.Object, _directory.Object, _analyser.Object, _scriptLocator.Object, _logger.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadDefaultScript()
        {
            CommandArguments args = new CommandArguments();
            _scriptLocator.Setup(x => x.FindBuildScript(args)).Returns("e.cs");

            _fileLoader.Setup(i => i.ReadAllLines("e.cs"))
                .Returns(new List<string>
                {
                    "using System;",
                    "using FlubuCore.Context;",
                    "using FlubuCore.Scripting;",
                    string.Empty,
                    "public class MyBuildScript : DefaultBuildScript",
                    "{",
                    "    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)",
                    "    {",
                    "        System.Console.WriteLine(\"2222\");",
                    "        }",
                    string.Empty,
                    "        protected override void ConfigureTargets(ITaskContext context)",
                    "        {",
                    "            Console.WriteLine(\"2222\");",
                    "        }",
                    "    }"
                });

            _analyser.Setup(i => i.Analyze(It.IsAny<List<string>>()))
                .Returns(new AnalyserResult() { ClassName = "MyBuildScript" });

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync(args);
            var provider = new ServiceCollection().BuildServiceProvider();

            t.Run(new TaskSession(
                null,
                new TargetTree(provider, new CommandArguments()),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new FluentInterfaceFactory(provider),
                new BuildPropertiesSession(),
                new BuildSystem()));
        }

        [Fact]
        public async Task LoadSimpleScript()
        {
            CommandArguments args = new CommandArguments();
            _scriptLocator.Setup(x => x.FindBuildScript(args)).Returns("e.cs");
            _fileLoader.Setup(i => i.ReadAllLines("e.cs"))
                .Returns(new List<string>
                {
                    "using FlubuCore.Scripting;",
                    "using System;",
                    "using FlubuCore.Context;",
                    string.Empty,
                    "public class MyBuildScript : IBuildScript",
                    "{",
                    "    public int Run(ITaskSession session)",
                    "    {",
                    "        Console.WriteLine(\"11\");",
                    "        return 0;",
                    "    }",
                    "}",
                });

            _analyser.Setup(i => i.Analyze(It.IsAny<List<string>>()))
                .Returns(new AnalyserResult() { ClassName = "MyBuildScript" });

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync(args);

            var provider = new ServiceCollection().BuildServiceProvider();

            t.Run(new TaskSession(
                null,
                new TargetTree(provider, new CommandArguments()),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new FluentInterfaceFactory(provider),
                new BuildPropertiesSession(),
                new BuildSystem()));
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadDefaultScriptWithAnotherClass()
        {
            CommandArguments args = new CommandArguments();
            _scriptLocator.Setup(x => x.FindBuildScript(args)).Returns("e.cs");
            _fileLoader.Setup(i => i.ReadAllLines("e.cs"))
                .Returns(new List<string>
                {
                    "using System;",
                    "using FlubuCore.Context;",
                    "using FlubuCore.Scripting;",
                    string.Empty,
                    "public class MyBuildScript : DefaultBuildScript",
                    "{",
                    "    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)",
                    "    {",
                    "        System.Console.WriteLine(\"2222\");",
                    "        }",
                    string.Empty,
                    "        protected override void ConfigureTargets(ITaskContext context)",
                    "        {",
                    "            var test = new Test();",
                    "        }",
                    "    }",
                    "public class Test",
                    "{",
                    "}"
                });

            _analyser.Setup(i => i.Analyze(It.IsAny<List<string>>()))
                .Returns(new AnalyserResult() { ClassName = "MyBuildScript" });

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync(args);
            var provider = new ServiceCollection().BuildServiceProvider();

            t.Run(new TaskSession(
                null,
                new TargetTree(provider, new CommandArguments()),
                new CommandArguments(),
                new DotnetTaskFactory(provider),
                new FluentInterfaceFactory(provider),
                new BuildPropertiesSession(),
                new BuildSystem()));
        }
    }
}