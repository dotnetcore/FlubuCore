using System;

namespace FlubuCore.Commanding
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
