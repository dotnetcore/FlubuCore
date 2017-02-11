using System.Threading.Tasks;
using FlubuCore.Scripting;

namespace FlubuCore.Runner.Scripting
{
    public interface IScriptLoader
    {
        Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(string fileName);
    }
}
