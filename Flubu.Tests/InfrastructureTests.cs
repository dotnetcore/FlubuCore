using System;
using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore.Extensions;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Process;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Flubu.Tests
{
    public class InfrastructureTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        private readonly IServiceProvider _provider;

        public InfrastructureTests()
        {
            _services
                .AddCoreComponents()
                .AddCommandComponents()
                .AddArguments(new string[] { })
                .AddTasks();

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