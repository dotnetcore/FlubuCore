using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.Iis.Interfaces;

namespace FlubuCore.Context.FluentInterface.Interfaces
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
