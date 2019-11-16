using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using Moq;

//#nuget: Moq, 4.8.2
namespace FlubuCore.WebApi.Tests
{
    public class SimpleScript : DefaultBuildScript
    {
        public string RequiredParam { get; set; }

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

            session.CreateTarget("RequiredTarget")
                .Requires(() => RequiredParam);

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

            session.CreateTarget("WhenTarget")
                .Do(SuccesfullTarget, "file2.txt")
                .Group(
                    target =>
                    {
                        target.Do(SuccesfullTarget, "file.txt");
                    },
                    when: c => { return false; })
                .Group(target =>
                    {
                        target.Do(SuccesfullTarget, "file3.txt");
                    },
                    when: c => { return true; });

            Mock<ITaskContext> context = new Mock<ITaskContext>();
            Mock<ITaskFluentInterface> tf = new Mock<ITaskFluentInterface>();
            context.Setup(x => x.Tasks()).Returns(tf.Object);
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
