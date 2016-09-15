using System.Collections.Generic;
using flubu.Targeting;

namespace flubu.Scripting
{
    public class ExampleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(TaskSession session)
        {
            System.Console.WriteLine("configure props");
        }

        protected override void ConfigureTargets(TargetTree targetTree, ICollection<string> args)
        {
            System.Console.WriteLine("configure targets");
        }
    }
}
