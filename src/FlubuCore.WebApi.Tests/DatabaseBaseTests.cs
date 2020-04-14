using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LiteDB;

namespace FlubuCore.WebApi.Tests
{
    public class DatabaseBaseTests
    {
        public DatabaseBaseTests()
        {
            try
            {
                LiteRepository = new LiteRepository("Filename=database.db;Connection=Shared");
            }
            catch (LiteException ex)
            {
                if (ex.Message.Contains("File is not a valid LiteDB database format", StringComparison.OrdinalIgnoreCase))
                {
                    // This exception could be thrown due to LiteDB packages is updated to a newer version. So delete old file and try again.
                    File.Delete("database.db");
                    LiteRepository = new LiteRepository("Filename=database.db;Connection=Shared");
                }
            }

            LiteRepository.Database.DropCollection("users");
        }

        ~DatabaseBaseTests()
        {
            LiteRepository.Dispose();
        }

        protected LiteRepository LiteRepository { get; }
    }
}
