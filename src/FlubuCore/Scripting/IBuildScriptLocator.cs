using System.Threading.Tasks;

namespace FlubuCore.Scripting
{
    public interface IBuildScriptLocator
    {
        string FindBuildScript(CommandArguments args);

        string FindFlubuFile();
    }
}
