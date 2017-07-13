using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FlubuCore.Commanding;
using FlubuCore.Scripting;
using FlubuCore.WebApi.Controllers.Exception;
using FlubuCore.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
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
            _commandArguments.MainCommand = request.MainCommand;
            _commandArguments.Script = request.ScriptFilePathLocation;
            _commandArguments.RemainingCommands = request.RemainingCommands;
            var result = await _commandExecutor.ExecuteAsync();
            switch (result)
            {
                case 0:
                    return Ok();
                case StatusCodes.BuildScriptNotFound:
                {
                    string errorMsg;
                    if (string.IsNullOrEmpty(request.ScriptFilePathLocation))
                    {
                       errorMsg = "The build script file was not specified. Please specify it as the first argument or use some of the default paths for script file";
                    }
                    else
                    {
                        errorMsg = $"The build script at the location {request.ScriptFilePathLocation} was not found.";
                    }

                    throw new HttpError(HttpStatusCode.BadRequest, Model.ErrorCodes.ScriptNotFound, errorMsg);
                }
            }

            throw new HttpError(HttpStatusCode.InternalServerError, result.ToString());
        }
    }
}
