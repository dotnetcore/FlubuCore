using FlubuCore.Tasks.Iis;
using FlubuCore.Tasks.Iis.Interfaces;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IIisTaskFluentInterface
    {
        /// <summary>
        /// Task creates new web site in iis.
        /// </summary>
        /// <returns></returns>
        ICreateWebsiteTask CreateWebsiteTask();

        /// <summary>
        /// Task created new web application on the specified web site in iis.
        /// </summary>
        /// <returns></returns>
        ICreateWebApplicationTask CreateWebApplicationTask(string webApplcationName);

        /// <summary>
        /// Task creates new application pool in iis.
        /// </summary>
        /// <returns></returns>
        ICreateAppPoolTask CreateAppPoolTask(string applicationPoolName);

        /// <summary>
        /// Task delentes specified Application pool.
        /// </summary>
        /// <returns></returns>
        IDeleteAppPoolTask DeleteAppPoolTask(string appPoolName);

        /// <summary>
        /// /Task for controlling the application pool (start, stop).
        /// </summary>
        /// <returns></returns>
        IControlAppPoolTask ControlAppPoolTask(string applicationPoolName, ControlApplicationPoolAction action);

        /// <summary>
        /// Task adds binding to existing web site.
        /// </summary>
        /// <returns></returns>
        IAddWebsiteBindingTask AddWebsiteBindingTask();
    }
}
