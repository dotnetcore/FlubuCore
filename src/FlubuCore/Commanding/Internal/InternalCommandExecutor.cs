using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlubuCore.Commanding.Internal
{
    public abstract partial class InternalCommandExecutor
    {
        public static void ExecuteInternalCommand(List<string> commands)
        {
            string mainCommand = commands.First();

            switch (mainCommand)
            {
                case InternalFlubuCommands.Setup:
                {
                    SetupFlubu();
                    break;
                }
            }
        }
    }
}
