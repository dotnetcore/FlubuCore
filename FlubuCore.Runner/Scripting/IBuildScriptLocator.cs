using System.Threading.Tasks;
using FlubuCore.Scripting;

namespace FlubuCore.Runner.Scripting
{
    public interface IBuildScriptLocator
    {
        Task<IBuildScript> FindBuildScript(CommandArguments args);
    }
}
