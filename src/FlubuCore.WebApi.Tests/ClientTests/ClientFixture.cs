using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Services;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using LiteDB;
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
            _hashService = new HashService();
            LiteRepository = new LiteRepository("Filename=database.db;Upgrade=true;Connection=Shared");
            _repository = new UserRepository(LiteRepository);
            var hashedPassword = _hashService.Hash("password");
            LiteRepository.Database.DropCollection("users");
            _repository.AddUser(new User
            {
                Username = "User",
                Password = hashedPassword
            });

            Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        public TestServer Server { get; }

        public LiteRepository LiteRepository { get; }

        public void Dispose()
        {
            Server?.Dispose();
            LiteRepository?.Dispose();
        }
    }
}
