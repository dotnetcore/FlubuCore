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

        public ICreateWebApplicationTask CreateWebApplicationTask(string webApplcationName)
        {
            return Context.CreateTask<CreateWebApplicationTask>(webApplcationName);
        }

        public ICreateAppPoolTask CreateAppPoolTask(string appPoolName)
        {
            return Context.CreateTask<CreateAppPoolTask>(appPoolName);
        }

        public IDeleteAppPoolTask DeleteAppPoolTask()
        {
            return Context.CreateTask<DeleteAppPoolTask>();
        }

        public IControlAppPoolTask ControlAppPoolTask(string applicationPoolName, ControlApplicationPoolAction action)
        {
            return Context.CreateTask<ControlAppPoolTask>(applicationPoolName, action);
        }

        public IAddWebsiteBindingTask AddWebsiteBindingTask()
        {
            return Context.CreateTask<AddWebsiteBindingTask>();
        }
    }
}
