using System;
using FlubuCore.Targeting;

namespace FlubuCore.Tasks.NetCore
{
    public static class DotnetEf
    {
        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ITargetInternal DotnetAddEfMigration(this ITargetInternal target, string workingFolder, string migrationName = "default")
        {
            target.AddTask(null, AddEfMigration(workingFolder, migrationName));

            return target;
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ITargetInternal DotnetRemoveEfMigration(this ITargetInternal target, string workingFolder, bool forceRemove = true)
        {
            target.AddTask(null, RemoveEfMigration(workingFolder, forceRemove));

            return target;
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ITargetInternal DotnetEfUpdateDatabase(this ITargetInternal target, string workingFolder)
        {
            target.AddTask(null, EfUpdateDatabase(workingFolder));

            return target;
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ITargetInternal DotnetEfDropDatabase(this ITargetInternal target, string workingFolder)
        {
            target.AddTask(null, EfDropDatabase(workingFolder));

            return target;
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ExecuteDotnetTask AddEfMigration(string workingFolder, string migrationName = "default")
        {
            return new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "add", migrationName);
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ExecuteDotnetTask RemoveEfMigration(string workingFolder, bool forceRemove = true)
        {
            var task = new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "remove");

            if (forceRemove)
                task.WithArguments("--force");

            return task;
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ExecuteDotnetTask EfUpdateDatabase(string workingFolder)
        {
            return new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "update");
        }

        [Obsolete("User CoreTaskExtension fluent interface")]
        public static ExecuteDotnetTask EfDropDatabase(string workingFolder)
        {
            return new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "drop", "--force");
        }
    }
}
