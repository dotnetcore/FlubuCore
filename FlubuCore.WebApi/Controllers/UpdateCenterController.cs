using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Octokit;

namespace FlubuCore.WebApi.Controllers
{
    [Route("[controller]")]
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

            var model = new UpdateCenter()
            {
                LatestVersion = releases.TagName,
                CurrentVersion = currentVersion
            };

            return View(model);
        }
    }
}
