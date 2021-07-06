namespace FlubuCore.LiteDb.Infrastructure
{
    using System;

    public class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}
