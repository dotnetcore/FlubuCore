using System.Threading.Tasks;
using FlubuCore.Scripting;

namespace DotNet.Cli.Flubu.Scripting
{
    public interface IBuildScriptLocator
    {
        Task<IBuildScript> FindBuildScript(CommandArguments args);
    }
}
