using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Models;

namespace FlubuCore.WebApi.Repository
{
    public interface IUserRepository
    {
	    Task<List<User>> ListUsers();

	    Task AddUser(User user);
    }
}
