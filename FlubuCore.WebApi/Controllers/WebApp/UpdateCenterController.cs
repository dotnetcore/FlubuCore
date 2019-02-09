using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Octokit;

namespace FlubuCore.WebApi.Controllers.WebApp
{
    [Route("[controller]")]
#if !NETCOREAPP1_1
    ////[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
#else
     [Authorize(ActiveAuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
#endif

    public class UpdateCenterController : Controller
    {
        private readonly string _owner = "flubu-core";

        private readonly string _reponame = "flubu.core";

        private readonly IGitHubClient _client;

        public UpdateCenterController(IGitHubClient client)
        {
            _client = client;
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

        [HttpGet("Update")]
        public async Task Update()
        {
            string frameworkName = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isNetCore = frameworkName.StartsWith(".NETCoreApp");

            var latestRelease = await _client.Repository.Release.GetLatest(_owner, _reponame);
            List<ReleaseAsset> assets;
            if (isWindows)
            {
                assets = latestRelease.Assets.Where(x => x.Name.Contains("Windows")).ToList();
            }
            else
            {
                assets = latestRelease.Assets.Where(x => x.Name.Contains("Linux")).ToList();
            }

            ReleaseAsset asset = null;

            if (isNetCore)
            {
                string netCoreVersion = frameworkName.Replace(".NETCoreApp,Version=", string.Empty);
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
                asset = assets.FirstOrDefault(x => x.Name.Contains("Net462"));
            }
#if !NETCOREAPP1_1
            var wc = new WebClient();
            var filename = "Release.zip";
            await wc.DownloadFileTaskAsync(asset.BrowserDownloadUrl, filename);
#endif
        }
    }
}
