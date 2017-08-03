using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
        public async void ExecuteScript_ExecuteSimpleScript_Sucesfull()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
			if (File.Exists("test.txt"))
            File.Delete("test.txt");

            Assert.False (File.Exists("test.txt"));

            Assert.False(File.Exists("test.txt"));
            await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "simplescript.cs",
                    TargetToExecute = "SuccesfullTarget"
                });

            Assert.True(File.Exists("test.txt"));
        }

        [Fact]
        public async void ExecuteScript_MainCommandEmpty_ThrowsBadRequest()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "bla"
                }));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal(ErrorCodes.ModelStateNotValid, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_ScriptNotFound_ThrowsBadRequest()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "somescript.cs",
                    TargetToExecute = "compile"
                }));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal(ErrorCodes.ScriptNotFound, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_TargetNotFound_ThrowsBadRequest()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "simplescript.cs",
                    TargetToExecute = "nonexist"
                }));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal(ErrorCodes.TargetNotFound, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_TargetThrowsException_ThrowsInternalServer()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFilePathLocation = "simplescript.cs",
                    TargetToExecute = "FailedTarget"
                }));

            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
            Assert.Equal(ErrorCodes.InternalServerError, exception.ErrorCode);
            Assert.Equal("Error message", exception.ErrorMessage);
        }
    }
}
