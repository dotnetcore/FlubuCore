using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FlubuCore.Services;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using FlubuCore.WebApi.Tests.ClientTests;
using Renci.SshNet;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class ScriptsClientTests : ClientBaseTests
    {
	    private IUserRepository repository;

	    private IHashService hashService;

		public ScriptsClientTests(ClientFixture clientFixture) : base(clientFixture)
        {
	        if (!Directory.Exists("Scripts"))
	        {
		        Directory.CreateDirectory("Scripts");
	        }
	        else
	        {
		        Directory.Delete("Scripts", true);
		        Directory.CreateDirectory("Scripts");
	        }

			if (File.Exists("Users.json"))
	        {
		        File.Delete("Users.json");
	        }

	        repository = new UserRepository();
	        hashService = new HashService();
	        var hashedPassword = hashService.Hash("password");
	        var result = repository.AddUser(new User
	        {
		        Username = "User",
		        Password = hashedPassword
	        });

	        result.GetAwaiter().GetResult();
		}

        [Fact]
        public async void ExecuteScript_ExecuteSimpleScript_Sucesfull()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;

	        await Client.UploadScriptAsync(new UploadScriptRequest
	        {
		        FilePath = "SimpleScript.cs"
	        });

			if (File.Exists("test.txt"))
            File.Delete("test.txt");

            Assert.False (File.Exists("test.txt"));

            Assert.False(File.Exists("test.txt"));
	        var req = new ExecuteScriptRequest
	        {
		        ScriptFileName = "simplescript.cs",
		        TargetToExecute = "SuccesfullTarget",
		        RemainingCommands = new List<string>(),
		        ScriptArguments = new Dictionary<string, string>()
	        };
			req.ScriptArguments.Add("FileName", "test.txt");
			await Client.ExecuteScriptAsync(req);

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
                    ScriptFileName = "bla"
                }));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal(ErrorCodes.ModelStateNotValid, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_ScriptNotFound_ThrowsBadRequest()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
	        var req = new ExecuteScriptRequest
	        {
		        ScriptFileName = "simplescript.cs",
		        TargetToExecute = "SuccesfullTarget",
		        RemainingCommands = new List<string>(),
		        ScriptArguments = new Dictionary<string, string>()
	        };
	        req.ScriptArguments.Add("FileName", "test.txt");
			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(req));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal(ErrorCodes.ScriptNotFound, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_TargetNotFound_ThrowsBadRequest()
        {
	        var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
	        Client.Token = token.Token;
	        await Client.UploadScriptAsync(new UploadScriptRequest
	        {
		        FilePath = "SimpleScript.cs"
	        });

			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFileName = "simplescript.cs",
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

	        await Client.UploadScriptAsync(new UploadScriptRequest
	        {
		        FilePath = "SimpleScript.cs"
	        });

			var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFileName = "simplescript.cs",
                    TargetToExecute = "FailedTarget"
                }));

            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
            Assert.Equal(ErrorCodes.InternalServerError, exception.ErrorCode);
            Assert.Equal("Error message", exception.ErrorMessage);
        }

	    [Fact]
	    public async void UploadScript_Succesfull()
	    {
		    var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
		    Client.Token = token.Token;

		    await Client.UploadScriptAsync(new UploadScriptRequest
		    {
			    FilePath = "SimpleScript.cs"
		    });

			Assert.True(File.Exists("Scripts\\SimpleScript.cs"));
	    }

	    [Fact]
	    public async void UploadScript_NotAllowedExtension_ThrowsForbiden()
	    {
		    var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
		    Client.Token = token.Token;

		    var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.UploadScriptAsync(new UploadScriptRequest
		    {
			    FilePath = "Users.json"
		    }));

			Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
			Assert.Equal("FileExtensionNotAllowed", exception.ErrorCode);
		    Assert.False(File.Exists("Scripts\\Users.json"));
	    }
	}
}
