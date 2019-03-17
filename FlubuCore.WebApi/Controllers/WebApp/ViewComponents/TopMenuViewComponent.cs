using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.WebApi.Configuration;
using FlubuCore.WebApi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlubuCore.WebApi.Controllers.WebApp.ViewComponents
{
    public class TopMenuViewComponent : ViewComponent
    {
        private WebAppSettings _webAppSettings;

        public TopMenuViewComponent(IOptions<WebAppSettings> webApiOptions)
        {
            _webAppSettings = webApiOptions.Value;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var model = new TopMenuViewModel();
            model.WebAppSettings = _webAppSettings;
            return Task.FromResult<IViewComponentResult>(View("_TopMenu", model));
        }
    }
}
