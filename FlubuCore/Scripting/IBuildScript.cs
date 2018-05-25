using FlubuCore.Context;
using FlubuCore.Targeting;

namespace FlubuCore.Scripting
{
    public interface IBuildScript
    {
        int Run(ITaskSession taskSession, ITargetCreator targetCreator = null);
    }
}