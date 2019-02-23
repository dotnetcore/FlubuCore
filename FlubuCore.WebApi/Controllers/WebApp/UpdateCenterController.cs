using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Packaging;
using FlubuCore.Tasks.Text;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Octokit;

namespace FlubuCore.WebApi.Controllers.WebApp
{
    [Route("[controller]")]
#if !NETCOREAPP1_1
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
#else
     [Authorize(ActiveAuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
#endif

    public class UpdateCenterController : Controller
    {
        private readonly string _owner = "flubu-core";

        private readonly string _reponame = "flubu.core";

        private readonly IGitHubClient _client;

        private readonly ITaskFactory _taskFactory;

        private readonly ITaskSession _taskSession;

        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly IApplicationLifetime _applicationLifetime;

        public UpdateCenterController(IGitHubClient client, ITaskFactory taskFactory, ITaskSession taskSession, IHostingEnvironment hostingEnvironment, IApplicationLifetime applicationLifetime)
        {
            _client = client;
            _taskFactory = taskFactory;
            _taskSession = taskSession;
            _hostingEnvironment = hostingEnvironment;
            _applicationLifetime = applicationLifetime;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var latestRelease = await _client.Repository.Release.GetLatest(_owner, _reponame);
            var currentVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            var latestVersion = latestRelease.TagName;
            string frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            var current = new Version(currentVersion);
            var latest = new Version(latestVersion);

            var model = new UpdateCenterModel()
            {
                LatestVersion = latestVersion,
                CurrentVersion = current.ToString(3),
                NewVersionExists = current < latest,
            };

            return View(model);
        }

        [HttpGet("Prepare")]
        public async Task<IActionResult> Prepare()
        {
            string frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()
                ?.FrameworkName;
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isNetCore = frameworkName.StartsWith(".NETCoreApp");

            var latestRelease = await _client.Repository.Release.GetLatest(_owner, _reponame);
            List<ReleaseAsset> assets;

            ReleaseAsset asset = null;

            if (isNetCore)
            {
                string netCoreVersion = frameworkName.Replace(".NETCoreApp,Version=", string.Empty);
                if (isWindows)
                {
                    assets = latestRelease.Assets.Where(x => x.Name.Contains("Windows")).ToList();
                }
                else
                {
                    assets = latestRelease.Assets.Where(x => x.Name.Contains("Linux")).ToList();
                }

                switch (netCoreVersion.ToLowerInvariant())
                {
                    case "v2.1":
                    {
                        asset = assets.FirstOrDefault(x => x.Name.Contains("NetCoreApp2.1"));
                        break;
                    }

                    case "v2.0":
                    {
                        asset = assets.FirstOrDefault(x => x.Name.Contains("NetCoreApp2.0"));
                        break;
                    }

                    case "v1.1":
                    {
                        asset = assets.FirstOrDefault(x => x.Name.Contains("NetCoreApp1.1"));
                        break;
                    }
                }
            }
            else
            {
                asset = latestRelease.Assets.FirstOrDefault(x => x.Name.Contains("Net462"));
            }

            var rootDir = _hostingEnvironment.ContentRootPath;
            if (!Directory.Exists(Path.Combine(rootDir, "Updates")))
            {
                Directory.CreateDirectory(Path.Combine(rootDir, "Updates"));
            }

            if (!Directory.Exists(Path.Combine(rootDir, "Updates/WebApi")))
            {
                Directory.CreateDirectory(Path.Combine(rootDir, "Updates/WebApi"));
            }

            var filename = Path.Combine(rootDir, "Updates/FlubuCoreWebApi_LatestRelease.zip");
#if !NETCOREAPP1_1
            var wc = new WebClient();
            await wc.DownloadFileTaskAsync(asset.BrowserDownloadUrl, filename);
#endif
            var unzipTask = _taskFactory.Create<UnzipTask>(filename, Path.Combine(rootDir, "Updates/WebApi"));
            unzipTask.Execute(_taskSession);

            _taskFactory
                .Create<UpdateJsonFileTask>(Path.Combine(rootDir, "Updates/WebApi/DeploymentConfig.json"))
                .Update("DeploymentPath", rootDir)
                .Update("IsUpdate", "true").Execute(_taskSession);

            return View();
        }

        [HttpPost]
        public IActionResult Restart()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            var rootDir = _hostingEnvironment.ContentRootPath;
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(Path.Combine(rootDir, "Updates/WebApi")),
                FileName = isWindows ? Path.Combine(rootDir, "FlubuCore.WebApi.Updater.exe") : "dotnet FlubuCore.WebApi.Updater.dll",
                Arguments = isWindows.ToString()
            });

            _applicationLifetime.StopApplication();

            return Ok();
        }
    }
}
