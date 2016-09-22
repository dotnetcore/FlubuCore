using System;
using Flubu.Commanding;
using Flubu.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Flubu.Tests
{
    public class InfrastructureTests
    {
        private readonly IServiceCollection services = new ServiceCollection();

        private readonly IServiceProvider provider;

        public InfrastructureTests()
        {
            services.RegisterAll();

            provider = services.BuildServiceProvider();
        }

        [Fact]
        public void Resolve()
        {
            var executor = provider.GetRequiredService<ICommandExecutor>();

            Assert.IsType<CommandExecutor>(executor);
        }
    }
}