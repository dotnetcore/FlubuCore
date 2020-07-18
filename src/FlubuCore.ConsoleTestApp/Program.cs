using FlubuCore.Scripting;

namespace FlubuCore.ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "Build", "--ci=jenkins" };
            var engine = new FlubuEngine();

            int scriptId = 0; // for easy switching test scripts.

            switch (scriptId)
            {
                case 0:
                    engine.RunScript<DefaultTestScript>(args);
                    break;
                case 1:
                    engine.RunScript<TestScriptWithAsync>(new string[] { });
                    break;
            }


        }
    }
}
