using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public interface ICreateAppPoolTask
    {
        string ApplicationPoolName { get; set; }

        bool ClassicManagedPipelineMode { get; set; }

        CreateApplicationPoolMode Mode { get; set; }
    }
}
