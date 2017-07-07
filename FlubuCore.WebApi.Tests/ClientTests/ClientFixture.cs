using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    public class ClientFixture : IDisposable
    {
        public TestServer Server { get; }

        public ClientFixture()
        {
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}
