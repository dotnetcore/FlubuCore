using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class ScriptsClientTests : ClientBaseTests
    {
        public ScriptsClientTests(ClientFixture clientFixture)
            : base(clientFixture)
        {
            if (Directory.Exists("Scripts"))
            {
                Directory.Delete("Scripts", true);
            }

            Directory.CreateDirectory("Scripts");
        }

        [Fact]
        public async void ExecuteScript_ExecuteSimpleScript_Sucesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            if (File.Exists("test.txt"))
                File.Delete("test.txt");

            Assert.False(File.Exists("test.txt"));

            Assert.False(File.Exists("test.txt"));
            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "SuccesfullTarget",
                ScriptArguments = new Dictionary<string, string>()
            };
            req.ScriptArguments.Add("FileName", "test.txt");
            var response = await Client.ExecuteScriptAsync(req);
            Assert.InRange(response.Logs.Count, 11, 12);
            Assert.True(File.Exists("test.txt"));
        }

        [Fact]
        public async Task ExecuteScript_ExecuteSimpleScript_NonExistTarget()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            if (File.Exists("test.txt"))
                File.Delete("test.txt");

            Assert.False(File.Exists("test.txt"));

            Assert.False(File.Exists("test.txt"));
            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "NonExist",
                ScriptArguments = new Dictionary<string, string>()
            };

           var ex = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(req));
           Assert.Equal(ErrorCodes.TargetNotFound, ex.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_OnErrorTest_Sucesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            if (File.Exists("test.txt"))
                File.Delete("test.txt");

            if (File.Exists("OnError.txt"))
                File.Delete("OnError.txt");

            if (File.Exists("Finally.txt"))
                File.Delete("Finally.txt");

            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "OnErrorTarget",
                ScriptArguments = new Dictionary<string, string>()
            };

            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(req));
            Assert.True(File.Exists("Finally.txt"));
            Assert.True(File.Exists("OnError.txt"));
        }

        [Fact]
        public async void ExecuteScript_OnFinallyTest_Sucesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            if (File.Exists("Finally.txt"))
                File.Delete("Finally.txt");

            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "FinallyTarget",
                ScriptArguments = new Dictionary<string, string>()
            };

            await Client.ExecuteScriptAsync(req);
            Assert.True(File.Exists("Finally.txt"));
        }

        [Fact]
        public async void ExecuteScript_RequiresTest_Sucesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "RequiredTarget",
                ScriptArguments = new Dictionary<string, string>()
            };

            var ex = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(req));
            Assert.Equal("Target 'RequiredTarget' requires build script member 'RequiredParam' not to be null.", ex.ErrorMessage);
        }

        [Fact]
        public async void ExecuteScript_MainCommandEmpty_ThrowsBadRequest()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
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
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "SuccesfullTarget",
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
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFileName = "SimpleScript.cs",
                    TargetToExecute = "nonexist"
                }));

            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.Equal(ErrorCodes.TargetNotFound, exception.ErrorCode);
        }

        [Fact]
        public async void ExecuteScript_TargetThrowsException_ThrowsInternalServer()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.ExecuteScriptAsync(
                new ExecuteScriptRequest
                {
                    ScriptFileName = "SimpleScript.cs",
                    TargetToExecute = "FailedTarget"
                }));

            Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
            Assert.Equal(ErrorCodes.InternalServerError, exception.ErrorCode);
            Assert.Equal("Error message", exception.ErrorMessage);
            Assert.InRange(exception.Logs.Count, 11, 13);
        }

        [Fact]
        public async void ExecuteScript_WhenTest_Sucesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            if (File.Exists("file.txt"))
                File.Delete("file.txt");

            if (File.Exists("file2.txt"))
                File.Delete("file2.txt");

            if (File.Exists("file3.txt"))
                File.Delete("file3.txt");

            Assert.False(File.Exists("file.txt"));
            Assert.False(File.Exists("file2.txt"));
            Assert.False(File.Exists("file3.txt"));

            var req = new ExecuteScriptRequest
            {
                ScriptFileName = "SimpleScript.cs",
                TargetToExecute = "WhenTarget",
                ScriptArguments = new Dictionary<string, string>()
            };
            req.ScriptArguments.Add("FileName", "test.txt");
            var response = await Client.ExecuteScriptAsync(req);

            Assert.False(File.Exists("file.txt"));
            Assert.True(File.Exists("file2.txt"));
            Assert.True(File.Exists("file3.txt"));
        }

        [Fact]
        public async void UploadScript_Succesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadScriptAsync(new UploadScriptRequest
            {
                FilePath = "SimpleScript.cs"
            });

            Assert.True(File.Exists("Scripts/SimpleScript.cs"));
        }

        [Fact]
        public async void UploadScript_NotAllowedExtension_ThrowsForbiden()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            File.Delete("Users.Json");
            File.WriteAllText("Users.json", "test");
            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.UploadScriptAsync(
                new UploadScriptRequest
                {
                    FilePath = "Users.json"
                }));

            Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
            Assert.Equal("FileExtensionNotAllowed", exception.ErrorCode);
            Assert.False(File.Exists("Scripts/Users.json"));
        }
    }
}
