using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FlubuCore.WebApi.Controllers.WebApp
{
    [Route("[controller]")]
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
#if NETCOREAPP1_1
             return View("NotSupported");
#else
            //var claims = new List<Claim>()
            //{
            //};

            //var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //var userPrincipal = new ClaimsPrincipal(userIdentity);
            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userPrincipal));
            return View();
#endif
        }
    }
}
