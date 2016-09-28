using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.IO
{
    public interface IPathBuilder
    {
        string FileName { get; }

        int Length { get; }
    }
}
