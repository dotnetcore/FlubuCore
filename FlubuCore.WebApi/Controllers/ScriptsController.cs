using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Commanding;
////using DotNet.Cli.Flubu.Commanding;
using FlubuCore.Scripting;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ScriptsController : Controller
    {
        private ICommandExecutor _commandExecutor;

        private CommandArguments _commandArguments;

        public ScriptsController(ICommandExecutor commandExecutor, CommandArguments commandArguments)
        {
            _commandExecutor = commandExecutor;
            _commandArguments = commandArguments;
        }

        [HttpGet("Execute")]
        public async Task<IActionResult> Execute()
        {
            _commandArguments.MainCommand = "build";
            await _commandExecutor.ExecuteAsync();

            return Ok();
        }
    }
}
