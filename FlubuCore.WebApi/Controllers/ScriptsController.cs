using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Infrastructure;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Attributes;
using FlubuCore.WebApi.Controllers.Exceptions;
using FlubuCore.WebApi.Model;
using FlubuCore.WebApi.Repository;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FileMode = System.IO.FileMode;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ScriptsController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private ICommandExecutor _commandExecutor;

        private CommandArguments _commandArguments;

        private IHostingEnvironment _hostingEnvironment;

        private WebApiSettings _webApiSettings;

        public ScriptsController(
            IRepositoryFactory repositoryFactory,
            ICommandExecutor commandExecutor,
            CommandArguments commandArguments,
            IHostingEnvironment hostingEnvironment,
            IOptions<WebApiSettings> webApiOptions)
        {
            _repositoryFactory = repositoryFactory;
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
            _hostingEnvironment = hostingEnvironment;
            _webApiSettings = webApiOptions.Value;
        }

        [HttpPost("Execute")]
        [EmailNotificationFilter(NotificationFilter.ExecuteScript)]
        public async Task<IActionResult> Execute([FromBody] ExecuteScriptRequest request)
        {
            PrepareCommandArguments(request);

            try
            {
                var result = await _commandExecutor.ExecuteAsync();
                ExecuteScriptResponse response = new ExecuteScriptResponse { Logs = await GetLogs() };
                switch (result)
                {
                    case 0:
                        return Ok(response);
                }

                throw new HttpError(HttpStatusCode.InternalServerError, result.ToString()) { Logs = await GetLogs() };
            }
            catch (BuildScriptLocatorException e)
            {
                throw new HttpError(HttpStatusCode.BadRequest, ErrorCodes.ScriptNotFound, e.Message);
            }
            catch (TargetNotFoundException e)
            {
                throw new HttpError(HttpStatusCode.BadRequest, ErrorCodes.TargetNotFound, e.Message);
            }
        }

        private async Task<List<string>> GetLogs()
        {
            await Task.Delay(2000);
            return _repositoryFactory.CreateSerilogRepository().GetExecuteScriptLogs(HttpContext.TraceIdentifier);
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadScript()
        {
            if (!_webApiSettings.AllowScriptUpload)
            {
                throw new HttpError(HttpStatusCode.Forbidden);
            }

            if (!Request.HasFormContentType)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "FormHasNoContentType");
            }

            var form = Request.Form;

            if (form == null || form.Files.Count == 0)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "NoFiles");
            }

            var uploads = Path.Combine(_hostingEnvironment.ContentRootPath, "scripts");

            var script = form.Files[0];

            if (script.Length > 0)
            {
                var fileExtension = Path.GetExtension(script.FileName);
                if (fileExtension != ".cs")
                {
                    throw new HttpError(HttpStatusCode.Forbidden, "FileExtensionNotAllowed",
                        $"File extension {fileExtension} not allowed.");
                }

                using (var fileStream = new FileStream(Path.Combine(uploads, script.FileName), FileMode.Create))
                {
                    await script.CopyToAsync(fileStream);
                }
            }

            return Ok();
        }

        private void PrepareCommandArguments(ExecuteScriptRequest request)
        {
            _commandArguments.MainCommands = new List<string>();
            var scriptFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "scripts", request.ScriptFileName);
            _commandArguments.MainCommands.Add(request.TargetToExecute);
            _commandArguments.Script = scriptFullPath;
            if (request.ScriptArguments != null && request.ScriptArguments.Count > 0)
            {
                _commandArguments.ScriptArguments = new DictionaryWithDefault<string, string>(null);
                foreach (var scriptArgument in request.ScriptArguments)
                {
                    _commandArguments.ScriptArguments.Add(scriptArgument.Key, scriptArgument.Value);
                }
            }

            _commandArguments.RethrowOnException = true;
        }
    }
}
