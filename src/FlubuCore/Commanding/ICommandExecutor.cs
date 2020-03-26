using System.Threading.Tasks;

namespace FlubuCore.Commanding
{
    public interface ICommandExecutor
    {
        Task<int> ExecuteAsync();
    }
}
