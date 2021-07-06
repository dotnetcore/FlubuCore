namespace FlubuCore.LiteDb.Repository.Exceptions
{
    using System;

    public class NotUniqueException : Exception
    {
        public NotUniqueException(string message)
            : base(message)
        {
        }
    }
}
