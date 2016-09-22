using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flubu.Scripting;

namespace Flubu.Commanding
{
    public interface IFlubuCommandParser
    {
        CommandArguments Parse(string[] args);

        void ShowHelp();
    }
}
