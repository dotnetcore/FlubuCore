using System;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Commanding;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Process;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FlubuCore.Tests
{
    public class InfrastructureTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        private readonly IServiceProvider _provider;

        public InfrastructureTests()
        {
            _services
                .AddCoreComponents()
                .AddScriptAnalyzers()
                .AddFlubuLogging(_services)
                .AddCommandComponents(true)
                .AddParserComponents()
                .AddTasks();

            _services.AddSingleton(new CommandArguments());

            _provider = _services.BuildServiceProvider();
        }

        [Fact]
        public void ResolveCommandExecutor()
        {
            var executor = _provider.GetRequiredService<ICommandExecutor>();

            Assert.IsType<CommandExecutor>(executor);
        }

        [Fact]
        public void ResolveRunProgramTask()
        {
            IServiceCollection sc = new ServiceCollection();
            sc.AddTransient<ICommandFactory, CommandFactory>();

            IServiceProvider pr = sc.BuildServiceProvider();
            IRunProgramTask instance = ActivatorUtilities.CreateInstance<RunProgramTask>(pr, "test");
            Assert.IsType<RunProgramTask>(instance);
        }
    }
}