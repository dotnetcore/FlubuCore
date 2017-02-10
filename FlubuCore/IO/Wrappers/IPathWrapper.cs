using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.IO.Wrappers
{
    public interface IPathWrapper
    {
        string GetExtension(string path);
    }
}
