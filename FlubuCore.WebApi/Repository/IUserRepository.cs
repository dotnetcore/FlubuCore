using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Models;

namespace FlubuCore.WebApi.Repository
{
    public interface IUserRepository
    {
       List<User> ListUsersAsync();

       void AddUser(User user);
    }
}
