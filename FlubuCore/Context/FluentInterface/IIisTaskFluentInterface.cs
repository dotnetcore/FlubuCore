using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.Iis.Interfaces;

namespace FlubuCore.Context.FluentInterface
{
    public interface IIisTaskFluentInterface
    {
        TaskContext Context { get; set; }

        ICreateWebsiteTask CreateWebsiteTask();

        ICreateWebApplicationTask CreateWebApplicationTask();

        ICreateAppPoolTask CreateAppPoolTask();

        IDeleteAppPoolTask DeleteAppPoolTask();

        IControlAppPoolTask ControlAppPoolTask();

        IAddWebsiteBindingTask AddWebsiteBindingTask();
    }
}
