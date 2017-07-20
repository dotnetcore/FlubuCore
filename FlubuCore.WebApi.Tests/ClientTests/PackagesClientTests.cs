using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Model;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class PackagesClientTests : ClientBaseTests
    {
        public PackagesClientTests(ClientFixture clientFixture) : base(clientFixture)
        {
        }

        [Fact]
        public async Task Upload1PackageWithSearch_Succesfull()
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

            await Client.UploadPackageAsync(new UploadPackageRequest { DirectoryPath = "TestData", PackageSearchPattern = "SimpleScript2.zip" });

            Assert.True(File.Exists("Packages\\SimpleScript2.zip"));
            Assert.False(File.Exists("Packages\\SimpleScript.zip"));
        }

        [Fact]
        public async Task Upload2Packages_Succesfull()
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

            await Client.UploadPackageAsync(new UploadPackageRequest { DirectoryPath = "TestData" });

            Assert.True(File.Exists("Packages\\SimpleScript2.zip"));
            Assert.True(File.Exists("Packages\\SimpleScript.zip"));
        }
    }
}
