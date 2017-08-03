using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository.Exceptions;
using Newtonsoft.Json;

namespace FlubuCore.WebApi.Repository
{
    public class UserRepository : IUserRepository
    {
	    public async Task<List<User>> ListUsers()
	    {
		    if (!File.Exists("Users.json"))
		    {
			    throw new FileNotFoundException();
		    }

			using (var reader = File.OpenText("Users.json"))
			{
				string json = await reader.ReadToEndAsync();
			    return  JsonConvert.DeserializeObject<List<User>>(json);
			}
	    }

	    public async Task AddUser(User user)
	    {
		    string newJson;

		    if (File.Exists("Users.json"))
		    {

			    FileStream fileStream = new FileStream("Users.json", FileMode.Open);
			    using (StreamReader r = new StreamReader(fileStream))
			    {
				    string json = await r.ReadToEndAsync();

				    List<User> persons = JsonConvert.DeserializeObject<List<User>>(json);
				    if (persons.Exists(x => x.Username == user.Username))
				    {
					    throw new NotUniqueException($"Username {user.Username} already exists. ");
				    }

				    persons.Add(user);
				    newJson = JsonConvert.SerializeObject(persons);
			    }
		    }
		    else
		    {
				List<User> persons = new List<User>();
			    persons.Add(user);
			    newJson = JsonConvert.SerializeObject(persons);
			}

		    File.WriteAllText("Users.json", newJson);
		}
    }
}
