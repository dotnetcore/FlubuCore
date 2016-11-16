using FlubuCore.Context;

namespace FlubuCore.Scripting
{
    public interface IBuildScript
    {
        void Run(ITaskSession taskSession);
    }
}