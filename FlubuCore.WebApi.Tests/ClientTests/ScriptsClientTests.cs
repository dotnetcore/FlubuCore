using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Tests.ClientTests;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class ScriptsClientTests : ClientBaseTests
    {
        public ScriptsClientTests(ClientFixture clientFixture) : base(clientFixture)
        {
        }

        [Fact]
        public async void ExecuteScript_MainCommandEmpty_ThrowsBadRequest()
        {
           await Client.ExecuteScriptAsync(new ExecuteScriptRequest
            {
                ScriptFilePathLocation = "bla"
            });
        }
    }
}
