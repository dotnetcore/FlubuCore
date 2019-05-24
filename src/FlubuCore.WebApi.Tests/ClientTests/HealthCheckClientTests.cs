using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class HealthCheckClientTests : ClientBaseTests
    {
        public HealthCheckClientTests(ClientFixture clientFixture)
            : base(clientFixture)
        {
        }

        [Fact]
        public async Task GetHealthCheckReport()
        {
            await Client.HealthCheckAsync();
        }
    }
}
