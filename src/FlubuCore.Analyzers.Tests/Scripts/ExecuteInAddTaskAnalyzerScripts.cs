namespace FlubuCore.Analyzers.Tests.Scripts
{
    public static class ExecuteInAddTaskAnalyzerScripts
    {
        public const string CorrectAddCoreTaskScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class BuildTask : ITask
    {
        public int Execute(ITaskContext context) { return 0; }
        public BuildTask Configuration(string config) { return this; }
    }

    public interface ICoreTaskFluentInterface
    {
        BuildTask Build();
    }

    public static class TargetExtensions
    {
        public static void AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task) { }
    }

    public class SimpleScript
    {
        public void ConfigureTargets(ICoreTaskFluentInterface coreTask)
        {
            TargetExtensions.AddCoreTask(x => x.Build().Configuration(""Release""));
        }
    }
}";

        public const string ExecuteInAddCoreTaskScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class BuildTask : ITask
    {
        public int Execute(ITaskContext context) { return 0; }
    }

    public interface ICoreTaskFluentInterface
    {
        BuildTask Build();
    }

    public static class TargetExtensions
    {
        public static void AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task) { }
    }

    public class SimpleScript
    {
        private ITaskContext _context;

        public void ConfigureTargets(ICoreTaskFluentInterface coreTask)
        {
            TargetExtensions.AddCoreTask(x => x.Build().Execute(_context));
        }
    }
}";

        public const string ExecuteInAddTaskScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class RunProgramTask : ITask
    {
        public int Execute(ITaskContext context) { return 0; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public interface ITaskFluentInterface
    {
        RunProgramTask RunProgramTask(string program);
    }

    public static class TargetExtensions
    {
        public static void AddTask(Func<ITaskFluentInterface, ITask> task) { }
    }

    public class SimpleScript
    {
        private ITaskContext _context;

        public void ConfigureTargets(ITaskFluentInterface taskFluent)
        {
            TargetExtensions.AddTask(x => x.RunProgramTask(""tool.exe"").Execute(_context));
        }
    }
}";

        public const string ExecuteInAddCoreTaskAsyncScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class BuildTask : ITask
    {
        public int Execute(ITaskContext context) { return 0; }
    }

    public interface ICoreTaskFluentInterface
    {
        BuildTask Build();
    }

    public static class TargetExtensions
    {
        public static void AddCoreTaskAsync(Func<ICoreTaskFluentInterface, ITask> task) { }
    }

    public class SimpleScript
    {
        private ITaskContext _context;

        public void ConfigureTargets(ICoreTaskFluentInterface coreTask)
        {
            TargetExtensions.AddCoreTaskAsync(x => x.Build().Execute(_context));
        }
    }
}";

        public const string ExecuteInAddTaskAsyncScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class RunProgramTask : ITask
    {
        public int Execute(ITaskContext context) { return 0; }
    }

    public interface ITaskFluentInterface
    {
        RunProgramTask RunProgramTask(string program);
    }

    public static class TargetExtensions
    {
        public static void AddTaskAsync(Func<ITaskFluentInterface, ITask> task) { }
    }

    public class SimpleScript
    {
        private ITaskContext _context;

        public void ConfigureTargets(ITaskFluentInterface taskFluent)
        {
            TargetExtensions.AddTaskAsync(x => x.RunProgramTask(""tool.exe"").Execute(_context));
        }
    }
}";

        public const string ExecuteAsyncInAddCoreTaskScript = @"
using System;
using System.Threading.Tasks;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class BuildTask : ITask
    {
        public Task<int> ExecuteAsync(ITaskContext context) { return Task.FromResult(0); }
    }

    public interface ICoreTaskFluentInterface
    {
        BuildTask Build();
    }

    public static class TargetExtensions
    {
        public static void AddCoreTask(Func<ICoreTaskFluentInterface, ITask> task) { }
    }

    public class SimpleScript
    {
        private ITaskContext _context;

        public void ConfigureTargets(ICoreTaskFluentInterface coreTask)
        {
            TargetExtensions.AddCoreTask(x => x.Build().ExecuteAsync(_context));
        }
    }
}";

        public const string ExecuteInDoMethodScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }
    public interface ITaskContext { }

    public class BuildTask : ITask
    {
        public int Execute(ITaskContext context) { return 0; }
    }

    public interface ICoreTaskFluentInterface
    {
        BuildTask Build();
    }

    public class SimpleScript
    {
        public void TargetMerge(ITaskContext context, ICoreTaskFluentInterface coreTask)
        {
            coreTask.Build().Execute(context);
        }
    }
}";
    }
}
