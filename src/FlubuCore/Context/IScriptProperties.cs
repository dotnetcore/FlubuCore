using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Infrastructure.Terminal;
using FlubuCore.Scripting;

namespace FlubuCore.Context
{
    public interface IScriptProperties
    {
        void InjectProperties(IBuildScript buildScript, IFlubuSession flubuSession);

        List<Hint> GetPropertiesHints(IBuildScript buildScript);

        Dictionary<string, IReadOnlyCollection<Hint>> GetEnumHints(IBuildScript buildScript, IFlubuSession flubuSession);

        List<string> GetPropertiesHelp(IBuildScript buildScript);
    }
}
