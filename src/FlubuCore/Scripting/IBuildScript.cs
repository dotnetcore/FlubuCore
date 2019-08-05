using FlubuCore.Context;

namespace FlubuCore.Scripting
{
    public interface IBuildScript
    {
        int Run(IFlubuSession flubuSession);
    }
}