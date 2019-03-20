using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlubuCore.WebApi.Models.ViewModels
{
    public class ScriptsViewModel
    {
        public SelectList Scripts { get; set; }

        public SelectList Targets { get; set; }
    }
}
