using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [CollectionDefinition("Client tests")]
    public class ClientCollection : ICollectionFixture<ClientFixture>
    {
    }
}
