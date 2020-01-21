using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Scripting
{
    public interface IScriptFactory
    {
        IScriptProperties CreateScriptProperties();
    }
}
