using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlubuCore.Scripting
{
    public class ScriptProvider : IScriptProvider
    {
        private readonly IScriptLoader _scriptLoader;

        private IBuildScript _script = null;

        public ScriptProvider(IScriptLoader scriptLoader)
        {
            _scriptLoader = scriptLoader;
        }

        public async Task<IBuildScript> GetBuildScriptAsync(CommandArguments commandArguments, bool forceReload = false)
        {
            if (_script != null && !forceReload)
            {
                return _script;
            }

            _script = await _scriptLoader.FindAndCreateBuildScriptInstanceAsync(commandArguments);

            return _script;
        }
    }
}
