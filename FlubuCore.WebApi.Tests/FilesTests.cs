using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.WebApi.Infrastructure;
using Xunit;

namespace Flubu.Tests
{
    public class FilesTests
    {
        [Theory]
        [InlineData("FileName=database.db; Password=fsafa")]
        [InlineData("FileName=database.db;Password=fsafa")]
        [InlineData("FileName=database.db;; Password=fsafa")]
        [InlineData("FileName=database.db Password=fsafa")]
        [InlineData("filename=database.db Password=fsafa")]
        [InlineData("FILENAME=database.db Password=fsafa")]
        [InlineData("Password=fsafa FileName=database.db;")]
        [InlineData("Password=fsafa; FileName=database.db")]
        [InlineData("FileName=database.db")]
        [InlineData("FileName=database.db;")]
        public void GetFileNameFromConnectionString_CorrectConnectionString_Succesfull(string connectionString)
        {
            var fileName = Files.GetFileNameFromConnectionString(connectionString);
            Assert.Equal("database.db", fileName);
        }

        [Theory]
        [InlineData("database.db; Password=fsafa")]
        [InlineData("database.db;")]
        [InlineData("FileNam=database.db;")]
        [InlineData("FileName=; Password")]
        [InlineData("FileName=    ; Password")]
        [InlineData("FileName Password")]
        [InlineData("")]
        [InlineData(null)]
        public void GetFileNameFromConnectionString_IncorectConnectionString_Succesfull(string connectionString)
        {
            var fileName = Files.GetFileNameFromConnectionString(connectionString);
            Assert.Null(fileName);
        }
    }
}
