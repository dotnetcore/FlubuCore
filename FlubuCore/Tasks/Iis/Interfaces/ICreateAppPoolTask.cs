using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public interface ICreateAppPoolTask : ITaskOfT<int>
    {
        /// <summary>
        /// Name of the application pool.
        /// </summary>
        string ApplicationPoolName { get; set; }

        /// <summary>
        /// If <c>true</c> application pool is in classic mode. Otherwise in integrated.
        /// </summary>
        bool ClassicManagedPipelineMode { get; set; }

        CreateApplicationPoolMode Mode { get; set; }
    }
}
