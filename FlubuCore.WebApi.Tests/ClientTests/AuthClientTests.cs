using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Services;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class AuthClientTests : ClientBaseTests
    {
        private IUserRepository _repository;

        private IHashService _hashService;

        public AuthClientTests(ClientFixture clientFixture)
            : base(clientFixture)
        {
            _repository = new UserRepository();
            _hashService = new HashService();
            if (File.Exists("Users.json"))
            {
                File.Delete("Users.json");
            }

            if (File.Exists("Security.json"))
            {
                File.Delete("Security.json");
            }
        }

        [Fact]
        public async Task GetTokenTest()
        {
            var hashedPassword = _hashService.Hash("password");
            await _repository.AddUserAsync(new User
            {
                Username = "User",
                Password = hashedPassword
            });
            var result = await Client.GetToken(new GetTokenRequest
            {
                Username = "User",
                Password = "password"
            });

            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task GetTokenWrongPassowrdTest()
        {
            var hashedPassword = _hashService.Hash("password");
            await _repository.AddUserAsync(new User
            {
                Username = "User",
                Password = hashedPassword
            });
            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.GetToken(
                new GetTokenRequest
                {
                    Username = "User",
                    Password = "Password"
                }));

            Assert.Equal(GetTokenRequest.WrongUsernamePassword, exception.ErrorCode);
        }

        [Fact]
        public async Task DisableGetTokenAfterToManyFailedGetTokenAttemptsTest()
        {
            var hashedPassword = _hashService.Hash("password");
            await _repository.AddUserAsync(new User
            {
                Username = "User",
                Password = hashedPassword
            });

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    await Client.GetToken(new GetTokenRequest
                    {
                        Username = "User",
                        Password = "Password"
                    });
                }
                catch (Exception)
                {
                }
            }

            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.GetToken(
                new GetTokenRequest
                {
                    Username = "User",
                    Password = "Password"
                }));

            Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        }

        [Fact]
        public async Task GetTokenWrongUsernameTest()
        {
            var hashedPassword = _hashService.Hash("password");
            await _repository.AddUserAsync(new User
            {
                Username = "User",
                Password = hashedPassword
            });
            var exception = await Assert.ThrowsAsync<WebApiException>(async () => await Client.GetToken(
                new GetTokenRequest
                {
                    Username = "User2",
                    Password = "password"
                }));

            Assert.Equal(GetTokenRequest.WrongUsernamePassword, exception.ErrorCode);
        }
    }
}
