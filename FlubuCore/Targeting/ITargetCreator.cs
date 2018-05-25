using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Targeting
{
    public interface ITargetCreator
    {
        void CreateTargetFromMethodAttributes(Type buildScriptType, ITaskSession taskSession);
    }
}
