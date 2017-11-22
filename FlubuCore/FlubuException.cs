using System;

namespace FlubuCore
{
    public abstract class FlubuException : Exception
    {
        protected FlubuException(string message) : base(message)
        { }

        protected FlubuException(string message, Exception inner) : base(message, inner)
        { }
    }
}
