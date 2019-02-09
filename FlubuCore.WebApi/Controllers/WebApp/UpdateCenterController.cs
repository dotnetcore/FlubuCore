using System;
using System.Reflection;
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
        private readonly IGitHubClient _client;

        public UpdateCenterController(IGitHubClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> Index()
        {
            var owner = "flubu-core";
            var reponame = "flubu.core";
            var releases = await _client.Repository.Release.GetLatest(owner, reponame);
            var currentVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            var latestVersion = releases.TagName;

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
    }
}
