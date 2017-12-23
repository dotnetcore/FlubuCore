using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.WebApi.Client;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    public class ClientBaseTests
    {
        public ClientBaseTests(ClientFixture clientFixture)
        {
            Client = new WebApiClient(clientFixture.Server.CreateClient());
            Server = clientFixture.Server;
        }

        protected TestServer Server { get; private set; }

        protected IWebApiClient Client { get; set; }
    }
}
