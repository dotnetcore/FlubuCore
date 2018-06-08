using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Packaging;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ITaskFactory _taskFactory;

        private readonly ITaskSession _taskSession;

        public ReportsController(IHostingEnvironment hostingEnvironment, ITaskFactory taskFactory, ITaskSession taskSession)
        {
            _hostingEnvironment = hostingEnvironment;
            _taskFactory = taskFactory;
            _taskSession = taskSession;
        }

        [HttpPost("download")]
        public IActionResult DownloadReports([FromBody]DownloadReportsRequest request)
        {
            string downloadDirectory = Path.Combine(_hostingEnvironment.ContentRootPath, "Reports");

            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            string dirName = "reports";

            if (!string.IsNullOrEmpty(request.DownloadFromSubDirectory))
            {
                downloadDirectory = Path.Combine(downloadDirectory, request.DownloadFromSubDirectory);
                dirName = request.DownloadFromSubDirectory;
            }

            var zipDirectory = Path.Combine(downloadDirectory, "output");

            var task = _taskFactory.Create<PackageTask>(zipDirectory);

            string zipFilename = string.IsNullOrEmpty(request.DownloadFromSubDirectory)
                ? "Reports.zip"
                : $"{request.DownloadFromSubDirectory}.zip";

            task.AddDirectoryToPackage(downloadDirectory, dirName, true).ZipPackage(zipFilename, false).Execute(_taskSession);

            Stream fs = System.IO.File.OpenRead(Path.Combine(zipDirectory, zipFilename));
            return File(fs, "application/zip", zipFilename);
        }
    }
}
