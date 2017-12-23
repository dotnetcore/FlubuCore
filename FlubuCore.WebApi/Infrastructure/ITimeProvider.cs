using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.WebApi.Infrastructure
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
