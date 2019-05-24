using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Models;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers.WebApp
{
    [Route("[controller]")]
#if !NETCOREAPP1_1
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
#else
    [Authorize(ActiveAuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
#endif
    public class ScriptController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ITargetExtractor _targetExtractor;

        private readonly ICommandExecutor _commandExecutor;

        private readonly CommandArguments _commandArguments;
        private WebAppSettings _webAppSettings
            ;

        public ScriptController(
            IHostingEnvironment hostingEnvironment,
            ITargetExtractor targetExtractor,
            ICommandExecutor commandExecutor,
            CommandArguments commandArguments,
            IOptions<WebAppSettings> webApiOptions)
        {
            _hostingEnvironment = hostingEnvironment;
            _targetExtractor = targetExtractor;
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
            _webAppSettings = webApiOptions.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!_webAppSettings.AllowScriptExecution)
            {
                return NotFound();
            }

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
            if (!_webAppSettings.AllowScriptExecution)
            {
                return NotFound();
            }

            var scriptsFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Scripts", scriptName);
            var targets = _targetExtractor.ExtractTargets(scriptsFullPath);
            return Ok(targets);
        }

        [HttpPost("Execute")]
        [EmailNotificationFilter(NotificationFilter.ExecuteScript)]
        public IActionResult Execute([FromBody]ExecuteScript request)
        {
            if (!_webAppSettings.AllowScriptExecution)
            {
                return NotFound();
            }

            PrepareCommandArguments(request.ScriptName, request.TargetName);

            try
            {
                var result = _commandExecutor.ExecuteAsync().Result;
                switch (result)
                {
                    case 0:
                    {
                        return Ok(new { msg = "Script executed successfully." });
                    }

                    default:
                    {
                        return Ok(new { msg = $"Script executed with code: {result}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { msg = ex.Message });
            }
        }

        private void PrepareCommandArguments(string scriptName, string targetName)
        {
            _commandArguments.MainCommands = new List<string>();
            var scriptFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Scripts", scriptName);
            _commandArguments.MainCommands.Add(targetName);
            _commandArguments.Script = scriptFullPath;
            _commandArguments.RethrowOnException = true;
        }
    }
}
