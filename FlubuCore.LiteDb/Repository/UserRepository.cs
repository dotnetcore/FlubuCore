namespace FlubuCore.LiteDb.Repository
{
    using System.Collections.Generic;

    using FlubuCore.LiteDb.Models;

    using LiteDB;

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
