using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Controllers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers
{
    [ServiceFilter(typeof(ValidateRequestModelAttribute))]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [ServiceFilter(typeof(RestrictApiAccessFilter))]
    public class ControllerBase : Controller
    {
    }
}
