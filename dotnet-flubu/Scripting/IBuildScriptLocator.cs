using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.Scripting
{
    public interface IBuildScriptLocator
    {
        Task<IBuildScript> FindBuildScript(CommandArguments args);
    }
}
