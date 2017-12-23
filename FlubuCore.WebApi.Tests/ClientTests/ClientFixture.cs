using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Services;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    public class ClientFixture : IDisposable
    {
        private IUserRepository _repository;

        private IHashService _hashService;

        public ClientFixture()
        {
            if (File.Exists("Users.json"))
            {
                File.Delete("Users.json");
            }

            _repository = new UserRepository();
            _hashService = new HashService();
            var hashedPassword = _hashService.Hash("password");
            var task = _repository.AddUserAsync(new User
            {
                Username = "User",
                Password = hashedPassword
            });
            task.Wait();
            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        public TestServer Server { get; }

        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}
