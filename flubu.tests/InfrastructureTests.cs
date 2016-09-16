using flubu.Commanding;
using flubu.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace flubu.tests
{
    public class InfrastructureTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();
        private IServiceProvider _provider;

        public InfrastructureTests()
        {
            _services.RegisterAll();

            _provider = _services.BuildServiceProvider();
        }

        [Fact]
        public void Resolve()
        {
            ICommandExecutor executor = _provider.GetRequiredService<ICommandExecutor>();

            Assert.IsType<CommandExecutor>(executor);
        }
    }
}