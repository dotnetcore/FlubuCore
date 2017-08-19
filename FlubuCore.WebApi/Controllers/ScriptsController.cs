using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
	public class ScriptsController : ControllerBase
    {
        private ICommandExecutor _commandExecutor;

        private CommandArguments _commandArguments;

        public ScriptsController(ICommandExecutor commandExecutor, CommandArguments commandArguments)
        {
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
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

	    private void PrepareCommandArguments(ExecuteScriptRequest request)
	    {
		    _commandArguments.MainCommand = request.TargetToExecute;
		    _commandArguments.Script = request.ScriptFilePathLocation;
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
