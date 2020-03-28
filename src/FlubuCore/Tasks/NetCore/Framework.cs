using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Infrastructure;

namespace FlubuCore.Tasks.NetCore
{
    public class Framework : Enumeration
    {
#pragma warning disable SA1310 // Field names should not contain underscore
        public static readonly Framework Net45 = new Framework(1, "net45");
        public static readonly Framework Net451 = new Framework(1, "net451");
        public static readonly Framework Net452 = new Framework(1, "net452");
        public static readonly Framework Net46 = new Framework(1, "net46");
        public static readonly Framework Net461 = new Framework(1, "net461");
        public static readonly Framework Net462 = new Framework(1, "net462");
        public static readonly Framework Net47 = new Framework(1, "net47");
        public static readonly Framework Net471 = new Framework(1, "net471");
        public static readonly Framework Net472 = new Framework(1, "net472");
        public static readonly Framework Net48 = new Framework(1, "net48");

        public static readonly Framework NetStandard14 = new Framework(1, "netstandard1.4");
        public static readonly Framework NetStandard15 = new Framework(1, "netstandard1.5");
        public static readonly Framework NetStandard16 = new Framework(1, "netstandard1.6");
        public static readonly Framework NetStandard20 = new Framework(1, "netstandard2.0");
        public static readonly Framework NetStandard21 = new Framework(1, "netstandard2.1");

        public static readonly Framework NetCoreApp20 = new Framework(1, "netcoreapp2.0");
        public static readonly Framework NetCoreApp21 = new Framework(1, "netcoreapp2.1");
        public static readonly Framework NetCoreApp22 = new Framework(1, "netcoreapp2.2");
        public static readonly Framework NetCoreApp30 = new Framework(1, "netcoreapp3.0");
        public static readonly Framework NetCoreApp31 = new Framework(1, "netcoreapp3.1");

#pragma warning restore SA1310 // Field names should not contain underscore

        public Framework(int id, string name)
            : base(id, name)
        {
        }
    }
}
