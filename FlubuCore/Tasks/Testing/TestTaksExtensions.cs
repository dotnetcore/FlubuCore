using System.IO;
using FlubuCore.Targeting;
using FlubuCore.Tasks.NetCore;

namespace FlubuCore.Tasks.Testing
{
    public static class TestTaksExtensions
    {
        public static ITarget UnitTest(ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                target.AddTask(Dotnet.Test(project));
            }

            return target;
        }

        public static ITarget Coverage(ITarget target, params string[] projects)
        {
            foreach (string project in projects)
            {
                OpenCoverTask task = new OpenCoverTask();

                target.AddTask(task
                    .Output($"{Path.GetFileNameWithoutExtension(project)}cover.xml")
                    .UseDotNet());
            }

            return target;
        }
    }
}
