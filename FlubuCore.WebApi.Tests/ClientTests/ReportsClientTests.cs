using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Model;
using Xunit;

namespace FlubuCore.WebApi.Tests.ClientTests
{
    [Collection("Client tests")]
    public class ReportsClientTests : ClientBaseTests
    {
        public ReportsClientTests(ClientFixture clientFixture)
            : base(clientFixture)
        {
            if (Directory.Exists("Reports"))
            {
                Directory.Delete("Reports", true);
            }

            Directory.CreateDirectory("Reports");
        }

        [Fact]
        public async Task DownloadReports_FromRoot_Succesfull()
        {
            using (File.Create("Reports\\test.txt"))
            {
            }

            using (File.Create("Reports\\test2.txt"))
            {
            }

            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            var report = await Client.DownloadReportsAsync(new DownloadReportsRequest
            {
            });

            Assert.True(report.Length > 100);
        }

        [Fact]
        public async Task DownloadReports_FromSubfolder_Succesfull()
        {
            Directory.CreateDirectory("Reports\\Diffs");
            using (File.Create("Reports\\Diffs\\test.txt"))
            {
            }

            using (File.Create("Reports\\Diffs\\test2.txt"))
            {
            }

            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            var report = await Client.DownloadReportsAsync(new DownloadReportsRequest
            {
                DownloadFromSubDirectory = "Diffs"
            }) as MemoryStream;

            Assert.True(report.Length > 100);

            FileStream file = new FileStream("k:\\test.zip", FileMode.Create, FileAccess.Write);
            report.WriteTo(file);
        }

        [Fact]
        public async Task DeleteReports_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (Directory.Exists("reports"))
            {
                Directory.Delete("reports", true);
            }

            Directory.CreateDirectory("Reports");
            using (File.Create("Reports/test.txt"))
            {
            }

            await Client.CleanReportsDirectoryAsync(new CleanReportsDirectoryRequest());
            Assert.False(File.Exists("Reports/test.txt"));
            Assert.True(Directory.Exists("Reports"));
        }

        [Fact]
        public async Task DeleteReports_SubFolder_Succesfull()
        {
            var token = await Client.GetToken(new GetTokenRequest { Username = "User", Password = "password" });
            Client.Token = token.Token;
            if (Directory.Exists("Reports"))
            {
                Directory.Delete("Reports", true);
            }

            Directory.CreateDirectory("Reports");
            Directory.CreateDirectory("Reports/subdir");
            using (File.Create("Reports/subdir/test.txt"))
            {
            }

            using (File.Create("Reports/ttt.txt"))
            {
            }

            await Client.CleanReportsDirectoryAsync(new CleanReportsDirectoryRequest()
            {
                SubDirectoryToDelete = "subdir"
            });
            Assert.False(File.Exists("Reports/subdir/test.txt"));
            Assert.True(File.Exists("Reports//ttt.txt"));
            Assert.True(Directory.Exists("Reports/subdir"));
        }
    }
}
