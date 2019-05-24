using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : Controller
    {
        private readonly ISecurityRepository _securityRepository;

        public HealthCheckController(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        /// <summary>
        /// Flubu web api healthcheck
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult HealthCheck()
        {
            _securityRepository.GetSecurity();
            return Ok();
        }
    }
}
