using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository.Exceptions;
using LiteDB;
using Newtonsoft.Json;
using FileMode = System.IO.FileMode;

namespace FlubuCore.WebApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly LiteRepository _repository;

        public UserRepository(LiteRepository repository)
        {
            _repository = repository;
        }

        public List<User> ListUsersAsync()
        {
            return _repository.Query<User>("users").ToList();
        }

        public void AddUser(User user)
        {
            _repository.Insert(user, "users");
        }
    }
}
