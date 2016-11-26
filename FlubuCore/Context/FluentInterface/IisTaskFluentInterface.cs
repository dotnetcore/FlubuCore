using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.Iis.Interfaces;

namespace FlubuCore.Context.FluentInterface
{
    public class IisTaskFluentInterface : IIisTaskFluentInterface
    {
        public TaskContext Context { get; set; }

        public ICreateWebsiteTask CreateWebsiteTask()
        {
            return Context.CreateTask<CreateWebsiteTask>();
        }

        public ICreateWebApplicationTask CreateWebApplicationTask()
        {
            return Context.CreateTask<CreateWebApplicationTask>();
        }

        public ICreateAppPoolTask CreateAppPoolTask()
        {
            return Context.CreateTask<CreateAppPoolTask>();
        }

        public IDeleteAppPoolTask DeleteAppPoolTask()
        {
            return Context.CreateTask<DeleteAppPoolTask>();
        }

        public IControlAppPoolTask ControlAppPoolTask()
        {
            return Context.CreateTask<ControlAppPoolTask>();
        }

        public IAddWebsiteBindingTask AddWebsiteBindingTask()
        {
            return Context.CreateTask<AddWebsiteBindingTask>();
        }
    }
}
