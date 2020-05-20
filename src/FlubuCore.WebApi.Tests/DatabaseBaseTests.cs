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
            LiteRepository = new LiteRepository("Filename=database.db;Upgrade=true;Connection=Shared");
            LiteRepository.Database.DropCollection("users");
        }

        ~DatabaseBaseTests()
        {
            LiteRepository.Dispose();
        }

        protected LiteRepository LiteRepository { get; }
    }
}
