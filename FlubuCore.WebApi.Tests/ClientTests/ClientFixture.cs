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
        public TestServer Server { get; }

	    private IUserRepository repository;

	    private IHashService hashService;

		public ClientFixture()
        {
	        if (File.Exists("Users.json"))
	        {
		        File.Delete("Users.json");
	        }

	        repository = new UserRepository();
	        hashService = new HashService();
	        var hashedPassword = hashService.Hash("password");
	        var task = repository.AddUser(new User
	        {
		        Username = "User",
		        Password = hashedPassword
	        });
	        task.Wait();
			Server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}
