using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Attributes;
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
    public class ScriptController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ITargetExtractor _targetExtractor;

        private readonly ICommandExecutor _commandExecutor;

        private readonly CommandArguments _commandArguments;

        public ScriptController(
            IHostingEnvironment hostingEnvironment,
            ITargetExtractor targetExtractor,
            ICommandExecutor commandExecutor,
            CommandArguments commandArguments)
        {
            _hostingEnvironment = hostingEnvironment;
            _targetExtractor = targetExtractor;
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
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

        [HttpGet("TargetNames")]
        public IActionResult GetTargetNamesFromScript(string scriptName)
        {
            var scriptsFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Scripts", scriptName);
            var targets = _targetExtractor.ExtractTargets(scriptsFullPath);
            return Ok(targets);
        }

        [HttpPost("Execute")]
        [EmailNotificationFilter(NotificationFilter.ExecuteScript)]
        public async Task<IActionResult> Execute([FromForm]ScriptsViewModel model)
        {
            PrepareCommandArguments(model);

            try
            {
                var result = await _commandExecutor.ExecuteAsync();
                switch (result)
                {
                    case 0:
                    {
                        model.ScriptExecutionMessage = "Script executed successfully.";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                model.ScriptExecutionMessage = ex.Message;
            }

            return View("Index", model);
        }

        private void PrepareCommandArguments(ScriptsViewModel model)
        {
            _commandArguments.MainCommands = new List<string>();
            var scriptFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Scripts", model.SelectedScript);
            _commandArguments.MainCommands.Add(model.SelectedTarget);
            _commandArguments.Script = scriptFullPath;
            _commandArguments.RethrowOnException = true;
        }
    }
}
