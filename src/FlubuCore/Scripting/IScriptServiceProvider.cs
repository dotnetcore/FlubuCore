using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Targeting;

namespace FlubuCore.Scripting
{
    public interface IScriptServiceProvider
    {
        IScriptProperties GetScriptProperties();

        ITargetCreator GetTargetCreator();

        FlubuConfiguration GetFlubuConfiguration();
    }
}
