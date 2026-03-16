using System.Threading.Tasks;

namespace FlubuCore.Tool
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await FlubuEntryPoint.MainAsync(args);
        }
    }
}
