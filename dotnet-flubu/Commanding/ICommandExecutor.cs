using System.Threading.Tasks;

namespace Flubu.Commanding
{
    public interface ICommandExecutor
    {
        Task<int> ExecuteAsync();
    }
}
