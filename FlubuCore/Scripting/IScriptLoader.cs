using System.Threading.Tasks;

namespace FlubuCore.Scripting
{
    public interface IScriptLoader
    {
        Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(CommandArguments args);
    }
}
