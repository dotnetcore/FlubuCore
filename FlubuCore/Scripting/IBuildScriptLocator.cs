using System.Threading.Tasks;

namespace FlubuCore.Scripting
{
    public interface IBuildScriptLocator
    {
        Task<IBuildScript> FindBuildScript(CommandArguments args);
    }
}
