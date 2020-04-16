using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Packaging;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#if NETCOREAPP3_1
    using Microsoft.Extensions.Hosting;
#else
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace FlubuCore.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IHostEnvironment _hostEnvironment;

        private readonly ITaskFactory _taskFactory;

        private readonly IFlubuSession _flubuSession;

        public ReportsController(IHostEnvironment hostEnvironment, ITaskFactory taskFactory, IFlubuSession flubuSession)
        {
            _hostEnvironment = hostEnvironment;
            _taskFactory = taskFactory;
            _flubuSession = flubuSession;
        }

        /// <summary>
        /// Sends reports(compressed in zip file) that are on the flubu web api server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("download")]
        public IActionResult DownloadReports([FromBody]DownloadReportsRequest request)
        {
            string downloadDirectory = Path.Combine(_hostEnvironment.ContentRootPath, "Reports");

            if (!Directory.Exists(downloadDirectory))
            {
                Directory.CreateDirectory(downloadDirectory);
            }

            string dirName = "Reports";

            if (!string.IsNullOrEmpty(request.DownloadFromSubDirectory))
            {
                var destDirPath = Path.GetFullPath(Path.Combine(downloadDirectory + Path.DirectorySeparatorChar));
                downloadDirectory = Path.GetFullPath(Path.Combine(downloadDirectory, request.DownloadFromSubDirectory));

                if (!downloadDirectory.StartsWith(destDirPath))
                {
                    throw new HttpError(HttpStatusCode.Forbidden);
                }

                dirName = request.DownloadFromSubDirectory;
            }

            var zipDirectory = Path.Combine(downloadDirectory, "output");

            var task = _taskFactory.Create<PackageTask>(zipDirectory);

            string zipFilename = string.IsNullOrEmpty(request.DownloadFromSubDirectory)
                ? "Reports.zip"
                : $"{request.DownloadFromSubDirectory}.zip";

            if (Directory.GetFiles(downloadDirectory).Length == 0)
            {
                throw new HttpError(HttpStatusCode.NotFound, "NoReportsFound");
            }

            task.AddDirectoryToPackage(downloadDirectory, dirName, false).ZipPackage(zipFilename, false).Execute(_flubuSession);
            string zipPath = Path.Combine(zipDirectory, zipFilename);

            Stream fs = System.IO.File.OpenRead(zipPath);
            return File(fs, "application/zip", zipFilename);
        }

        /// <summary>
        /// Deletes all reports(cleans directory on flubu web api server).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("download")]
        public IActionResult CleanPackagesDirectory([FromBody]CleanPackagesDirectoryRequest request)
        {
            var downloadDirectory = Path.Combine(_hostEnvironment.ContentRootPath, "Reports");

            if (!string.IsNullOrWhiteSpace(request.SubDirectoryToDelete))
            {
                var destDirPath = Path.GetFullPath(Path.Combine(downloadDirectory + Path.DirectorySeparatorChar));
                downloadDirectory = Path.GetFullPath(Path.Combine(downloadDirectory, request.SubDirectoryToDelete));

                if (!downloadDirectory.StartsWith(destDirPath))
                {
                    throw new HttpError(HttpStatusCode.Forbidden);
                }
            }

            try
            {
                if (Directory.Exists(downloadDirectory))
                {
                    Directory.Delete(downloadDirectory, true);
                }
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
                Directory.Delete(downloadDirectory, true);
            }

            Directory.CreateDirectory(downloadDirectory);

            return Ok();
        }
    }
}
