using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Scripting;

namespace FlubuCore.WebApi.Tests
{
    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
            session.CreateTarget("SuccesfullTarget").Do(SuccesfullTarget, "f",
                doOptions: x => x.ForMember(m => m.Param, "FileName"));
            session.CreateTarget("FailedTarget").Do(FailedTarget);

            session.CreateTarget("FinallyTarget")
                .Group(target =>
                    {
                        target.AddTask(x => x.DeleteDirectoryTask("fakeDir", false));
                    },
                    onFinally: c =>
                    {
                        File.Create("Finally.txt");
                    });

            session.CreateTarget("OnErrorTarget")
                .Group(target =>
                    {
                        target.Do(FailedTarget);
                    },
                    onFinally: c =>
                    {
                        File.Create("Finally.txt");
                    },
                    onError: (c, e) =>
                    {
                        File.Create("OnError.txt");
                    });
        }

        public void SuccesfullTarget(ITaskContext session, string fileName)
        {
            File.Create(fileName);
        }

        public void FailedTarget(ITaskContext session)
        {
            throw new TaskExecutionException("Error message", 5);
        }
    }
}
