namespace FlubuCore.LiteDb.Repository
{
    using System.Collections.Generic;

    using LiteDB;

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
