using System.Collections.Generic;

namespace flubu.Scripting
{
    public interface IBuildScript
    {
        int Run(ICollection<string> args);
    }
}