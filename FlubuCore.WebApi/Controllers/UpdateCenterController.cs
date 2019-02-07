using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [Route("[controller]")]
    public class UpdateCenterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
