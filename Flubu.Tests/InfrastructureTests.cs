using System;
using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
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
                .RegisterAll()
                .AddArguments(new string[] { });

            _provider = _services.BuildServiceProvider();
        }

        [Fact]
        public void Resolve()
        {
            var executor = _provider.GetRequiredService<ICommandExecutor>();

            Assert.IsType<CommandExecutor>(executor);
        }
    }
}