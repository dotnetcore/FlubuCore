using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Services;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class PackagesClientTests : ClientBaseTests
    {
        private readonly IUserRepository _repository;

        private readonly IHashService _hashService;

        public PackagesClientTests(ClientFixture clientFixture)
            : base(clientFixture)
        {
        }

        [Fact]
        public async Task Upload1PackageWithSearch_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            if (!Directory.Exists("Packages"))
            {
                Directory.CreateDirectory("Packages");
            }
            else
            {
                Directory.Delete("Packages", true);
                Directory.CreateDirectory("Packages");
            }

            await Client.UploadPackageAsync(new UploadPackageRequest
            {
                DirectoryPath = "TestData",
                PackageSearchPattern = "SimpleScript2.zip"
            });

            Assert.True(File.Exists("Packages/SimpleScript2.zip"));
            Assert.False(File.Exists("Packages/SimpleScript.zip"));
        }

        [Fact]
        public async Task Upload2Packages_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (!Directory.Exists("Packages"))
            {
                Directory.CreateDirectory("Packages");
            }
            else
            {
                Directory.Delete("Packages", true);
                Directory.CreateDirectory("Packages");
            }

            await Client.UploadPackageAsync(new UploadPackageRequest { DirectoryPath = "TestData" });

            Assert.True(File.Exists("Packages/SimpleScript2.zip"));
            Assert.True(File.Exists("Packages/SimpleScript.zip"));
        }

        [Fact]
        public async Task Upload1PackageToSubDirectory_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            if (!Directory.Exists("Packages"))
            {
                Directory.CreateDirectory("Packages");
            }
            else
            {
                Directory.Delete("Packages", true);
                Directory.CreateDirectory("Packages");
            }

            await Client.UploadPackageAsync(new UploadPackageRequest
            {
                DirectoryPath = "TestData",
                PackageSearchPattern = "SimpleScript2.zip",
                UploadToSubDirectory = "SomeSubdir"
            });

            Assert.True(File.Exists("Packages/SomeSubdir/SimpleScript2.zip"));
            Assert.False(File.Exists("Packages/SimpleScript2.zip"));
            Assert.False(File.Exists("Packages/SomeSubdir/SimpleScript.zip"));
            Assert.False(File.Exists("Packages/SimpleScript.zip"));
        }

        [Fact]
        public async Task DeletePackages_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (Directory.Exists("packages"))
            {
                Directory.Delete("Packages", true);
            }

            Directory.CreateDirectory("Packages");
            using (File.Create("packages/test.txt"))
            {
            }

            await Client.DeletePackagesAsync(new CleanPackagesDirectoryRequest());
            Assert.False(File.Exists("packages/test.txt"));
            Assert.True(Directory.Exists("Packages"));
        }

        [Fact]
        public async Task DeletePackages_SubFolder_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (Directory.Exists("packages"))
            {
                Directory.Delete("Packages", true);
            }

            Directory.CreateDirectory("Packages");
            Directory.CreateDirectory("Packages/subdir");
            using (File.Create("packages/subdir/test.txt"))
            {
            }

            using (File.Create("packages/ttt.txt"))
            {
            }

            await Client.DeletePackagesAsync(new CleanPackagesDirectoryRequest
            {
                SubDirectoryToDelete = "subdir"
            });
            Assert.False(File.Exists("packages/subdir/test.txt"));
            Assert.True(File.Exists("packages//ttt.txt"));
            Assert.True(Directory.Exists("Packages/subdir"));
        }
    }
}
