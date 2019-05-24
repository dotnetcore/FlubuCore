using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using LiteDB;
using Xunit;

namespace FlubuCore.WebApi.Tests
{
    public class UserRepositoryTests : DatabaseBaseTests
    {
        private IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            _userRepository = new UserRepository(LiteRepository);
        }

        [Fact]
        public void AddAndListUsersTest()
        {
            _userRepository.AddUser(new User
            {
                Username = "Test",
                Password = "beda",
            });

            _userRepository.AddUser(new User
            {
                Username = "Test2",
                Password = "beda",
            });

            var result = _userRepository.ListUsersAsync();
            Assert.Equal(2, result.Count);
            Assert.Equal("Test", result[0].Username);
            Assert.Equal("beda", result[0].Password);
            Assert.Equal("Test2", result[1].Username);
        }
    }
}
