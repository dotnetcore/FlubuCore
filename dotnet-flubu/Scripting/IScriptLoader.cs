using System.Threading.Tasks;

namespace Flubu.Scripting
{
    public interface IScriptLoader
    {
        Task<IBuildScript> FindAndCreateBuildScriptInstanceAsync(string fileName);
    }
}
