using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Infrastructure.Terminal
{
    public static class InternalTerminalCommands
    {
        public const string Cd = "cd";

        public const string CdBack = "cd..";

        public const string CdBackToDisk = "cd...";

        public const string Dir = "dir";

        public const string Cls = "cls";

        public static readonly List<string> InteractiveExitOnlyCommands = new List<string>()
        {
            "x",
            "exit",
            "q",
            "quit",
        };

        public static List<string> ReloadCommands => new List<string>
        {
            "r",
            "reload",
            "l",
            "load",
        };

        public static List<string> InteractiveExitAndReloadCommands
        {
            get
            {
                var ret = new List<string>();
                ret.AddRange(ReloadCommands);
                ret.AddRange(InteractiveExitOnlyCommands);
                return ret;
            }
        }
    }
}
