using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlubuCore.WebApi.Controllers.WebApp
{
    [Route("[controller]")]
////#if !NETCOREAPP1_1
////    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
////#else
////    [Authorize(ActiveAuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
////#endif
    public class ScriptsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ITargetExtractor _targetExtractor;

        public ScriptsController(IHostingEnvironment hostingEnvironment, ITargetExtractor targetExtractor)
        {
            _hostingEnvironment = hostingEnvironment;
            _targetExtractor = targetExtractor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var scriptsFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Scripts");

            if (!Directory.Exists(scriptsFullPath))
            {
                return View(new ScriptsViewModel()
                {
                    Scripts = new SelectList(new List<string>() { "No scripts found" })
                });
            }

            var scriptFiles = Directory.GetFiles(scriptsFullPath, "*.cs");
            List<string> scriptFileNames = scriptFiles.Select(sf => Path.GetFileName(sf)).ToList();
            List<string> targets = _targetExtractor.ExtractTargets(scriptFiles[0]);
            return View(new ScriptsViewModel
            {
                Scripts = new SelectList(scriptFileNames),
                Targets = new SelectList(targets),
            });
        }
    }
}
