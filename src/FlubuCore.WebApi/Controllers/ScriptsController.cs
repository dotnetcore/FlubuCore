using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#if NETCOREAPP3_1
    using Microsoft.Extensions.Hosting;
#else
    using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using FileMode = System.IO.FileMode;

namespace FlubuCore.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ScriptsController : ControllerBase
    {
        private readonly IRepositoryFactory _repositoryFactory;

        private readonly ILogger<ScriptsController> _logger;

        private readonly ICommandExecutor _commandExecutor;

        private readonly CommandArguments _commandArguments;

        private readonly IHostEnvironment _hostEnvironment;

        private readonly WebApiSettings _webApiSettings;

        public ScriptsController(
            IRepositoryFactory repositoryFactory,
            ICommandExecutor commandExecutor,
            CommandArguments commandArguments,
            IHostEnvironment hostingEnvironment,
            IOptions<WebApiSettings> webApiOptions,
            ILogger<ScriptsController> logger)
        {
            _repositoryFactory = repositoryFactory;
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
            _hostEnvironment = hostingEnvironment;
            _logger = logger;
            _webApiSettings = webApiOptions.Value;
        }

        /// <summary>
        /// Executes specified FlubuCore script.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Execute")]
        [SwaggerResponse(200, Type = typeof(ExecuteScriptResponse))]
        [SwaggerResponse(400, Type = typeof(ErrorModel), Description = "Error codes: ScriptNotFount, TargetNotFound")]
        [SwaggerResponse(401)]
        [SwaggerResponse(500, Type = typeof(ErrorModel), Description = "Internal Server error occured.")]
        [EmailNotificationFilter(NotificationFilter.ExecuteScript)]
        public async Task<IActionResult> Execute([FromBody] ExecuteScriptRequest request)
        {
            PrepareCommandArguments(request);

            try
            {
                var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
                _logger.LogInformation($"Flubu v.{version}");
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

        /// <summary>
        /// Uploads specified flubu script to flubu server.
        /// </summary>
        /// <returns></returns>
        [SwaggerResponse(200)]
        [SwaggerResponse(400, Type = typeof(ErrorModel), Description = "Error codes: FormHasNoContentType, NoFiles")]
        [SwaggerResponse(401)]
        [SwaggerResponse(403, Type = typeof(ErrorModel), Description = "Error codes: FileExtensionNotAllowed")]
        [SwaggerResponse(500, Type = typeof(ErrorModel), Description = "Internal Server error occured.")]
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

            var uploads = Path.Combine(_hostEnvironment.ContentRootPath, "Scripts");

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

        private async Task<List<string>> GetLogs()
        {
            if (!_webApiSettings.AddFlubuLogsToResponse)
            {
                return null;
            }

            await Task.Delay(3100);
            return _repositoryFactory.CreateSerilogRepository().GetExecuteScriptLogs(HttpContext.TraceIdentifier);
        }

        private void PrepareCommandArguments(ExecuteScriptRequest request)
        {
            _commandArguments.MainCommands = new List<string>();
            var scriptFullPath = Path.Combine(_hostEnvironment.ContentRootPath, "Scripts", request.ScriptFileName);
            _commandArguments.MainCommands.Add(request.TargetToExecute);
            _commandArguments.Script = scriptFullPath;
            _commandArguments.IsWebApi = true;
            if (request.ScriptArguments != null && request.ScriptArguments.Count > 0)
            {
                _commandArguments.ScriptArguments = new DictionaryWithDefault<string, string>(null, StringComparer.OrdinalIgnoreCase);
                foreach (var scriptArgument in request.ScriptArguments)
                {
                    _commandArguments.ScriptArguments.Add(scriptArgument.Key, scriptArgument.Value);
                }
            }

            _commandArguments.RethrowOnException = true;
        }
    }
}
