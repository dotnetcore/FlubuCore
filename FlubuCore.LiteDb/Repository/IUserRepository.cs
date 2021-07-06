namespace FlubuCore.LiteDb.Repository
{
    using System.Collections.Generic;

    using FlubuCore.LiteDb.Models;

    public interface IUserRepository
    {
       List<User> ListUsersAsync();

       void AddUser(User user);
    }
}
