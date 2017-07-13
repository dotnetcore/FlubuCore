using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.WebApi.Client;
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
            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "bla"
                }));

            Assert.Equal(ErrorCodes.ModelStateNotValid, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_ScriptNotFound_ThrowsBadRequest()
        {
            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "somescript.cs",
                    MainCommand = "compile"
                }));

            Assert.Equal(ErrorCodes.ScriptNotFound, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_ExecuteSimpleScript_Sucesfull()
        {
            await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "simplescript.cs",
                    MainCommand = "test"
                });
        }
    }
}
