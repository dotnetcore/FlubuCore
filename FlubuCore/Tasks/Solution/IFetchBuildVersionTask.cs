using System;

namespace FlubuCore.Tasks.Solution
{
    public interface IFetchBuildVersionTask : ITask
    {
        Version BuildVersion { get; }
    }
}