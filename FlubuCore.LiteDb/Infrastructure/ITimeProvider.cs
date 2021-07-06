namespace FlubuCore.LiteDb.Infrastructure
{
    using System;

    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
