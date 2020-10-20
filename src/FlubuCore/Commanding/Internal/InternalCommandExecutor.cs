using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore.Commanding.Internal
{
    public abstract partial class InternalCommandExecutor
    {
        protected InternalCommandExecutor(IFlubuSession flubuSession, CommandArguments args)
        {
            FlubuSession = flubuSession;
            Args = args;
        }

        public IFlubuSession FlubuSession { get; }

        public CommandArguments Args { get; protected set; }

        public async Task ExecuteInternalCommand()
        {
            string mainCommand = Args.MainCommands.First();

            switch (mainCommand)
            {
                case InternalFlubuCommands.Setup:
                {
                    SetupFlubu();
                    break;
                }

                case InternalFlubuCommands.New:
                {
                    await CreateNewProject();
                    break;
                }
            }
        }
    }
}
