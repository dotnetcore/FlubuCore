using Flubu.Tasks;

namespace Flubu.Scripting
{
    public interface IBuildScript
    {
        int Run(ITaskSession taskSession);
    }
}