using System;

namespace Flubu.Tasks.Solution
{
    public interface IFetchBuildVersionTask : ITask
    {
        Version BuildVersion { get; }
    }
}