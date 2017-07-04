using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Cli.Flubu.Commanding;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ScriptsController : Controller
    {
        private ICommandExecutor _commandExecutor;

        public ScriptsController(ICommandExecutor commandExecutor)
        {
            _commandExecutor = commandExecutor;
        }

        [HttpGet("Execute")]
        public async Task<IActionResult> Execute()
        {
            await _commandExecutor.ExecuteAsync();
            return Ok();
        }
    }
}
