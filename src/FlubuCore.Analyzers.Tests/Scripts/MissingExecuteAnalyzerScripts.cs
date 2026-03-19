namespace FlubuCore.Analyzers.Tests.Scripts
{
    public static class MissingExecuteAnalyzerScripts
    {
        public const string CorrectExecuteInChainScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public TaskFluentInterface Tasks() { return null; }
        public CoreTaskFluentInterface CoreTasks() { return null; }
    }

    public class RunProgramTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public class TaskFluentInterface
    {
        public RunProgramTask RunProgramTask(string program) { return null; }
    }

    public class CoreTaskFluentInterface
    {
        public BuildTask Build() { return null; }
    }

    public class BuildTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public BuildTask Configuration(string config) { return this; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            context.Tasks().RunProgramTask(""tool.exe"").WithArguments(""arg"").Execute(context);
        }
    }
}";

        public const string CorrectExecuteOnVariableScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public TaskFluentInterface Tasks() { return null; }
    }

    public class RunProgramTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public class TaskFluentInterface
    {
        public RunProgramTask RunProgramTask(string program) { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            var task = context.Tasks().RunProgramTask(""tool.exe"");
            task.WithArguments(""arg"").Execute(context);
        }
    }
}";

        public const string CorrectExecuteAfterFluentOnVariableScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public TaskFluentInterface Tasks() { return null; }
    }

    public class RunProgramTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public RunProgramTask WorkingFolder(string folder) { return this; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public class TaskFluentInterface
    {
        public RunProgramTask RunProgramTask(string program) { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            var progTask = context.Tasks().RunProgramTask(""tool.exe"");
            progTask
                .WorkingFolder(""bin"")
                .WithArguments(""arg"")
                .Execute(context);
        }
    }
}";

        public const string CorrectReassignedVariableScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public TaskFluentInterface Tasks() { return null; }
    }

    public class RunProgramTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public class TaskFluentInterface
    {
        public RunProgramTask RunProgramTask(string program) { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            var task = context.Tasks().RunProgramTask(""tool1.exe"");
            task.WithArguments(""arg"").Execute(context);

            task = context.Tasks().RunProgramTask(""tool2.exe"");
            task.Execute(context);
        }
    }
}";

        public const string MissingExecuteInChainScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public TaskFluentInterface Tasks() { return null; }
    }

    public class RunProgramTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public class TaskFluentInterface
    {
        public RunProgramTask RunProgramTask(string program) { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            context.Tasks().RunProgramTask(""tool.exe"").WithArguments(""arg"");
        }
    }
}";

        public const string MissingExecuteOnVariableScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public TaskFluentInterface Tasks() { return null; }
    }

    public class RunProgramTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public RunProgramTask WithArguments(string arg) { return this; }
    }

    public class TaskFluentInterface
    {
        public RunProgramTask RunProgramTask(string program) { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            var task = context.Tasks().RunProgramTask(""tool.exe"");
            task.WithArguments(""arg"");
        }
    }
}";

        public const string MissingExecuteDiscardedScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public CoreTaskFluentInterface CoreTasks() { return null; }
    }

    public class BuildTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
    }

    public class CoreTaskFluentInterface
    {
        public BuildTask Build() { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            context.CoreTasks().Build();
        }
    }
}";

        public const string MissingExecuteCoreTasksScript = @"
using System;

namespace TestNamespace
{
    public interface ITask { }

    public class TaskContext
    {
        public CoreTaskFluentInterface CoreTasks() { return null; }
    }

    public class BuildTask : ITask
    {
        public int Execute(TaskContext context) { return 0; }
        public BuildTask Configuration(string config) { return this; }
    }

    public class CoreTaskFluentInterface
    {
        public BuildTask Build() { return null; }
    }

    public class SimpleScript
    {
        public void TargetMethod(TaskContext context)
        {
            context.CoreTasks().Build().Configuration(""Release"");
        }
    }
}";
    }
}
