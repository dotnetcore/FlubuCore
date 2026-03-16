using System.Threading.Tasks;

namespace DotNet.Cli.Flubu
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await FlubuCore.FlubuEntryPoint.MainAsync(args);
        }
    }
}
