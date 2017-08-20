using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
	public class ScriptsController : ControllerBase
    {
        private ICommandExecutor _commandExecutor;

        private CommandArguments _commandArguments;

	    private IHostingEnvironment _hostingEnvironment;

		private WebApiSettings _webApiSettings;

        public ScriptsController(ICommandExecutor commandExecutor, CommandArguments commandArguments, IHostingEnvironment hostingEnvironment, IOptions<WebApiSettings> webApiOptions)
        {
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
	        _hostingEnvironment = hostingEnvironment;
	        _webApiSettings = webApiOptions.Value;
        }

        [HttpPost("Execute")]
        public async Task<IActionResult> Execute([FromBody] ExecuteScriptRequest request)
        {
	        PrepareCommandArguments(request);

	        try
            {
                var result = await _commandExecutor.ExecuteAsync();

                switch (result)
                {
                    case 0:
                        return Ok();
                }

                throw new HttpError(HttpStatusCode.InternalServerError, result.ToString());
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
					throw new HttpError(HttpStatusCode.Forbidden, "FileExtensionNotAllowed", $"File extension {fileExtension} not allowed.");
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
		    var scriptFullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "scripts", request.ScriptFileName);
			_commandArguments.MainCommand = request.TargetToExecute;
		    _commandArguments.Script = scriptFullPath;
		    _commandArguments.RemainingCommands = request.RemainingCommands;
		    if (request.ScriptArguments != null && request.ScriptArguments.Count > 0)
		    {
			    _commandArguments.ScriptArguments = new DictionaryWithDefault<string, string>(null);
			    foreach (var scriptArgument in request.ScriptArguments)
			    {
				    _commandArguments.ScriptArguments.Add(scriptArgument.Key, scriptArgument.Value);
			    }
		    }

		    _commandArguments.TreatUnknownTargetAsException = true;
		    _commandArguments.RethrowOnException = true;
	    }
    }
}
