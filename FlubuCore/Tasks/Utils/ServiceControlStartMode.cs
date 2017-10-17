using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Utils
{
    public enum ServiceControlStartMode
    {
        Boot,
        System,
        Auto,
        Demand,
        Disabled,
        DelayedAuto
    }
}
