using System.Threading.Tasks;

namespace DotNet.Cli.Flubu.Commanding
{
    public interface ICommandExecutor
    {
        Task<int> ExecuteAsync();
    }
}
