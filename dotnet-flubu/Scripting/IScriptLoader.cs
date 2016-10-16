using System.Threading.Tasks;
using FlubuCore.Scripting;

namespace DotNet.Cli.Flubu.Scripting
{
    public interface IScriptLoader
    {
        Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(string fileName);
    }
}
