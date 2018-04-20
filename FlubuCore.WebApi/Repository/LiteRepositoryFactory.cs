using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace FlubuCore.WebApi.Repository
{
    public class LiteRepositoryFactory : ILiteRepositoryFactory
    {
        private static readonly IDictionary<string, LiteDatabase> LiteDatabases = new Dictionary<string, LiteDatabase>();

        public LiteDatabase CreateLiteDatabase(string connectionString)
        {
            LiteDatabase liteDb;

            if (LiteDatabases.TryGetValue(connectionString, out liteDb))
            {
                return liteDb;
            }

            liteDb = new LiteDatabase(connectionString);

            LiteDatabases[connectionString] = liteDb;

            return liteDb;
        }
    }
}
