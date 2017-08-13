using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Xunit;

namespace FlubuCore.WebApi.Tests
{
    public class UserRepositoryTesst
    {
	    private IUserRepository userRepository;

	    public UserRepositoryTesst()
	    {
		    userRepository = new UserRepository();
			if (File.Exists("Users.json"))
			{
				File.Delete("Users.json");
			}
		}

	    [Fact]
	    public async void AddAndListUsersTest()
	    {
		    await userRepository.AddUser(new User
		    {
			    Username = "Test",
				Password = "beda",

		    });

		    await userRepository.AddUser(new User
		    {
			    Username = "Test2",
			    Password = "beda",

		    });

		    var result = await userRepository.ListUsers();
			Assert.Equal(2, result.Count);
			Assert.Equal("Test", result[0].Username);
		    Assert.Equal("beda", result[0].Password);
			Assert.Equal("Test2", result[1].Username);
		}
    }
}
