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

        public ICreateAppPoolTask CreateAppPoolTask(string applicationPoolName)
        {
            return Context.CreateTask<CreateAppPoolTask>(applicationPoolName);
        }

        public IDeleteAppPoolTask DeleteAppPoolTask(string appPoolName)
        {
            return Context.CreateTask<DeleteAppPoolTask>(appPoolName);
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
