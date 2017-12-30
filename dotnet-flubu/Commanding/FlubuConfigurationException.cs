using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore;

namespace DotNet.Cli.Flubu.Commanding
{
    public class FlubuConfigurationException : FlubuException
    {
        public FlubuConfigurationException(string message)
            : base(message)
        {
        }

        public FlubuConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
