using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.Scripting
{
    public interface IFileLoader
    {
        string LoadFile(string fileName);
    }
}
