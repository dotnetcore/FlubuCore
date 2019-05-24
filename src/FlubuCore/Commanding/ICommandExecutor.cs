using System.Threading.Tasks;

namespace FlubuCore.Commanding
{
    public interface ICommandExecutor
    {
        string FlubuHelpText { get; set; }

        Task<int> ExecuteAsync();
    }
}
