using System.Threading.Tasks;

namespace FlubuCore.Runner.Commanding
{
    public interface ICommandExecutor
    {
        Task<int> ExecuteAsync();
    }
}
