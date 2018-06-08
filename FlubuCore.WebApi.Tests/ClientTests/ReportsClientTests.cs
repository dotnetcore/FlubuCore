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

            var report = await Client.DownloadReports(new DownloadReportsRequest
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

            var report = await Client.DownloadReports(new DownloadReportsRequest
            {
                DownloadFromSubDirectory = "Diffs"
            }) as FileStream;

            Assert.True(report.Length > 100);
        }
    }
}
