using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Services;
using FlubuCore.WebApi.Client;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Repository;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class PackagesClientTests : ClientBaseTests
    {
        public PackagesClientTests(ClientFixture clientFixture)
            : base(clientFixture)
        {
            if (!Directory.Exists("Packages"))
            {
                Directory.CreateDirectory("Packages");
            }
            else
            {
                Directory.Delete("Packages", true);
                Directory.CreateDirectory("Packages");
            }
        }

        [Fact]
        public async Task Upload1PackageWithSearch_Succesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadPackageAsync(new UploadPackageRequest
            {
                DirectoryPath = "TestData",
                PackageSearchPattern = "SimpleScript2.zip",
            });

            Assert.True(File.Exists("Packages/SimpleScript2.zip"));
            Assert.False(File.Exists("Packages/SimpleScript.zip"));
        }

        [Fact]
        public async Task Upload2Packages_Succesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

            await Client.UploadPackageAsync(new UploadPackageRequest { DirectoryPath = "TestData" });

            Assert.True(File.Exists("Packages/SimpleScript2.zip"));
            Assert.True(File.Exists("Packages/SimpleScript.zip"));
        }

        [Fact]
        public async Task Upload1PackageToSubDirectory_Succesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;

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
        public async Task Upload1PackageToSubDirectory_DirectoryOutsideOfPackagesFolder_ThrowsForbiden()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest
            {
                Username = "User", Password = "password"
            });

            Client.Token = token.Token;

            var ex = await Assert.ThrowsAsync<WebApiException>(async () => await Client.UploadPackageAsync(
                new UploadPackageRequest
                {
                    DirectoryPath = "TestData",
                    PackageSearchPattern = "SimpleScript2.zip",
                    UploadToSubDirectory = "../../SomeSubdir"
                }));

            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);
        }

        [Fact]
        public async Task DeletePackages_Succesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (Directory.Exists("Packages"))
            {
                Directory.Delete("Packages", true);
            }

            Directory.CreateDirectory("Packages");
            using (File.Create("Packages/test.txt"))
            {
            }

            await Client.DeletePackagesAsync(new CleanPackagesDirectoryRequest());
            Assert.False(File.Exists("Packages/test.txt"));
            Assert.True(Directory.Exists("Packages"));
        }

        [Fact]
        public async Task DeletePackages_SubFolder_Succesfull()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (Directory.Exists("Packages"))
            {
                Directory.Delete("Packages", true);
            }

            Directory.CreateDirectory("Packages");
            Directory.CreateDirectory("Packages/subdir");
            using (File.Create("Packages/subdir/test.txt"))
            {
            }

            using (File.Create("Packages/ttt.txt"))
            {
            }

            await Client.DeletePackagesAsync(new CleanPackagesDirectoryRequest
            {
                SubDirectoryToDelete = "subdir"
            });
            Assert.False(File.Exists("Packages/subdir/test.txt"));
            Assert.True(File.Exists("Packages//ttt.txt"));
            Assert.True(Directory.Exists("Packages/subdir"));
        }

        [Fact]
        public async Task DeletePackagesFromSubDir_DirectoryOutsideOfPackagesFolder_ThrowsForbiden()
        {
            var token = await Client.GetTokenAsync(new GetTokenRequest
            {
                Username = "User", Password = "password"
            });

            Client.Token = token.Token;

            var ex = await Assert.ThrowsAsync<WebApiException>(async () => await Client.DeletePackagesAsync(
                new CleanPackagesDirectoryRequest()
                {
                    SubDirectoryToDelete = "../../SomeSubdir"
                }));

            Assert.Equal(HttpStatusCode.Forbidden, ex.StatusCode);
        }
    }
}
