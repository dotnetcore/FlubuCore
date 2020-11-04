using System;

namespace FlubuCore
{
    public class FlubuException : Exception
    {
        public FlubuException(string message)
            : base(message)
        { }

        public FlubuException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
