using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public interface ICreateAppPoolTask : ITaskOfT<int, CreateAppPoolTask>
    {
        /// <summary>
        /// Classic managed pipelinemode will be used instead of integrated managed pipelinemode.
        /// </summary>
        /// <returns></returns>
        ICreateAppPoolTask UseClassicManagedPipelineMode();

        /// <summary>
        /// Set the Managed runtime version(.net CLR version). By default latest is used.
        /// </summary>
        /// <param name="managedRuntimeVersion"></param>
        /// <returns></returns>
        ICreateAppPoolTask ManagedRuntimeVersion(string managedRuntimeVersion);

        ICreateAppPoolTask Mode(CreateApplicationPoolMode mode);
    }
}
