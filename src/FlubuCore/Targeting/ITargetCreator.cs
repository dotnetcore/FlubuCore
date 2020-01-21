using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore.Targeting
{
    public interface ITargetCreator
    {
        void CreateTargetFromMethodAttributes(IBuildScript buildScript, IFlubuSession flubuSession);
    }
}
