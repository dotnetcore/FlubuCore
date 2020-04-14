using FlubuCore.Scripting;

namespace FlubuCore.ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] {"Test"};

            var engine = new FlubuEngine();
            engine.RunScript<BuildScript>(args);
        }
    }
}
