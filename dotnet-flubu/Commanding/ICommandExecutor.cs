using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flubu.Commanding
{
    public interface ICommandExecutor
    {
        Task<int> Execute(string[] args);
    }
}
