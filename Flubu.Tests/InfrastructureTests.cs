using System;
using FlubuCore.Commanding;
using FlubuCore.Infrastructure;
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
                .AddArguments(new string[] { });

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