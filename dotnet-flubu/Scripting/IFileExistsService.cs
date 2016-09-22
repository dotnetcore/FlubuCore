using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.Scripting
{
    public interface IFileExistsService
    {
        bool FileExists(string fileName);
    }
}
