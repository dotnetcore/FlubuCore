using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Commanding
{
    public interface IFlubuCommandParserFactory
    {
        IFlubuCommandParser GetFlubuCommandParser();
    }
}
