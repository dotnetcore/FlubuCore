using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace FlubuCore.WebApi.Repository
{
    public interface ILiteRepositoryFactory
    {
        LiteDatabase CreateLiteDatabase(string connectionString);
    }
}
