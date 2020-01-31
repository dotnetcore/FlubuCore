using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FlubuCore.Scripting
{
    public interface IScriptProvider
    {
        Task<IBuildScript> GetBuildScriptAsync(CommandArguments commandArguments, bool forceReload = false);
    }
}
