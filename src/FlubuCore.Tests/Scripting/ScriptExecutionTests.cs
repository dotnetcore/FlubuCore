using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Infrastructure;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Scripting.Analysis;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class ScriptExecutionTests
    {
        private readonly Mock<IFileWrapper> _fileLoader = new Mock<IFileWrapper>();
        private readonly Mock<IDirectoryWrapper> _directory = new Mock<IDirectoryWrapper>();
        private readonly Mock<IScriptAnalyzer> _analyzer = new Mock<IScriptAnalyzer>();
        private readonly Mock<IBuildScriptLocator> _scriptLocator = new Mock<IBuildScriptLocator>();
        private readonly Mock<ILogger<ScriptLoader>> _logger = new Mock<ILogger<ScriptLoader>>();
        private readonly Mock<ILogger<FlubuSession>> _loggerTaskSession = new Mock<ILogger<FlubuSession>>();
        private readonly Mock<IProjectFileAnalyzer> _projectFileAnalyzer = new Mock<IProjectFileAnalyzer>();

        private readonly Mock<INugetPackageResolver> _nugetPackageResolver = new Mock<INugetPackageResolver>();
        private readonly ScriptLoader _loader;

        public ScriptExecutionTests()
        {
            _projectFileAnalyzer.Setup(x => x.Analyze(It.IsAny<string>(), It.IsAny<bool>())).Returns(new ProjectFileAnalyzerResult
            {
                ProjectFileFound = false
            });

            _nugetPackageResolver
                .Setup(x => x.ResolveNugetPackagesFromDirectives(It.IsAny<List<NugetPackageReference>>(), It.IsAny<string>())).Returns(new List<AssemblyInfo>());

            _loader = new ScriptLoader(_fileLoader.Object, _directory.Object, _projectFileAnalyzer.Object, _analyzer.Object, _scriptLocator.Object, _nugetPackageResolver.Object, _logger.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadDefaultScript()
        {
            var scriptFileName = "e.cs";

            var asmbFileName = _loader.GetBuildScriptAssemblyFileName(scriptFileName, fullPath: false);
            if (File.Exists(asmbFileName))
            {
                File.Delete(asmbFileName);
            }

            CommandArguments args = new CommandArguments();
            _scriptLocator.Setup(x => x.FindBuildScript(args)).Returns(scriptFileName);

            _fileLoader.Setup(i => i.ReadAllLines(scriptFileName))
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

            _analyzer.Setup(i => i.Analyze(It.IsAny<List<string>>()))
                .Returns(new ScriptAnalyzerResult() { ClassName = "MyBuildScript" });

            FlubuConfiguration configuration = new FlubuConfiguration();
            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync(args);
            var provider = new ServiceCollection()
                .AddSingleton<IScriptProperties, ScriptProperties>()
                .AddSingleton<ITargetCreator, TargetCreator>()
                .AddSingleton(configuration)
                .BuildServiceProvider();

            var targetTree = new TargetTree(provider, new CommandArguments());
            t.Run(new FlubuSession(
                _loggerTaskSession.Object,
                targetTree,
                new CommandArguments(),
                new ScriptServiceProvider(provider),
                new DotnetTaskFactory(provider),
                new FluentInterfaceFactory(provider),
                new BuildPropertiesSession(targetTree),
                new BuildServer()));
        }

        [Fact]
        public async Task LoadSimpleScript()
        {
            var scriptFileName = "e3.cs";

            var asmbFileName = _loader.GetBuildScriptAssemblyFileName(scriptFileName, fullPath: false);
            if (File.Exists(asmbFileName))
            {
                File.Delete(asmbFileName);
            }

            CommandArguments args = new CommandArguments();
            _scriptLocator.Setup(x => x.FindBuildScript(args)).Returns(scriptFileName);

            _fileLoader.Setup(i => i.ReadAllLines(scriptFileName))
                .Returns(new List<string>
                {
                    "using FlubuCore.Scripting;",
                    "using System;",
                    "using FlubuCore;",
                    "using FlubuCore.Context;",
                    "using Microsoft.Extensions.DependencyInjection;",
                    "using Microsoft.Extensions.Logging;",
                    string.Empty,
                    "public class MyBuildScript : IBuildScript",
                    "{",
                    "    public int Run(IFlubuSession session)",
                    "    {",
                    "        Console.WriteLine(\"11\");",
                    "        return 0;",
                    "    }",

                    "    public void ConfigureServices(IServiceCollection services)",
                    "    {",
                    "    }",

                    "    public void Configure(IFlubuConfigurationBuilder configurationBuilder, ILoggerFactory loggerFactory)",
                    "    {",
                    "    }",
                    "}",
                });

            _analyzer.Setup(i => i.Analyze(It.IsAny<List<string>>()))
                .Returns(new ScriptAnalyzerResult() { ClassName = "MyBuildScript" });

            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync(args);

            FlubuConfiguration configuration = new FlubuConfiguration();
            var provider = new ServiceCollection()
                .AddSingleton<IScriptProperties, ScriptProperties>()
                .AddSingleton<ITargetCreator, TargetCreator>()
                .AddSingleton(configuration)
                .BuildServiceProvider();

            var targetTree = new TargetTree(provider, new CommandArguments());

            t.Run(new FlubuSession(
                _loggerTaskSession.Object,
                targetTree,
                new CommandArguments(),
                new ScriptServiceProvider(provider),
                new DotnetTaskFactory(provider),
                new FluentInterfaceFactory(provider),
                new BuildPropertiesSession(targetTree),
                new BuildServer()));
        }

        [Fact]
        public async System.Threading.Tasks.Task LoadDefaultScriptWithAnotherClass()
        {
            var scriptFileName = "e2.cs";

            var asmbFileName = _loader.GetBuildScriptAssemblyFileName(scriptFileName, fullPath: false);
            if (File.Exists(asmbFileName))
            {
                File.Delete(asmbFileName);
            }

            CommandArguments args = new CommandArguments();
            _scriptLocator.Setup(x => x.FindBuildScript(args)).Returns(scriptFileName);
            _fileLoader.Setup(i => i.ReadAllLines(scriptFileName))
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

            _analyzer.Setup(i => i.Analyze(It.IsAny<List<string>>()))
                .Returns(new ScriptAnalyzerResult() { ClassName = "MyBuildScript" });
            FlubuConfiguration configuration = new FlubuConfiguration();
            IBuildScript t = await _loader.FindAndCreateBuildScriptInstanceAsync(args);
            var provider = new ServiceCollection()
                .AddSingleton<IScriptProperties, ScriptProperties>()
                .AddSingleton<ITargetCreator, TargetCreator>()
                .AddSingleton(configuration)
                .BuildServiceProvider();
            var targetTree = new TargetTree(provider, new CommandArguments());
            t.Run(new FlubuSession(
                _loggerTaskSession.Object,
                targetTree,
                new CommandArguments(),
                new ScriptServiceProvider(provider),
                new DotnetTaskFactory(provider),
                new FluentInterfaceFactory(provider),
                new BuildPropertiesSession(targetTree),
                new BuildServer()));
        }
    }
}