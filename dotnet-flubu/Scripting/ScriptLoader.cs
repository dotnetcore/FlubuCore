using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.IO;
using System.Threading.Tasks;

namespace flubu.Scripting
{
    public interface IScriptLoader
    {
        Task<IBuildScript> FindAndCreateBuildScriptInstance(string fileName);
    }

    public class ScriptLoader : IScriptLoader
    {
        public async Task<IBuildScript> FindAndCreateBuildScriptInstance(string fileName)
        {
            string code = File.ReadAllText(fileName);

            ScriptState<IBuildScript> res = await CSharpScript.RunAsync<IBuildScript>(code);

            return res.ReturnValue;
        }
    }
}