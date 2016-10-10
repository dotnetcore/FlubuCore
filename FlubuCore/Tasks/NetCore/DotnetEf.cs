using FlubuCore.Targeting;

namespace FlubuCore.Tasks.NetCore
{
    public static class DotnetEf
    {
        public static ITarget DotnetAddEfMigration(this ITarget target, string workingFolder, string migrationName = "default")
        {
            target.AddTask(AddEfMigration(workingFolder, migrationName));

            return target;
        }

        public static ITarget DotnetRemoveEfMigration(this ITarget target, string workingFolder, bool forceRemove = true)
        {
            target.AddTask(RemoveEfMigration(workingFolder, forceRemove));

            return target;
        }

        public static ITarget DotnetEfUpdateDatabase(this ITarget target, string workingFolder)
        {
            target.AddTask(EfUpdateDatabase(workingFolder));

            return target;
        }

        public static ExecuteDotnetTask AddEfMigration(string workingFolder, string migrationName = "default")
        {
            return new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "add", migrationName);
        }

        public static ExecuteDotnetTask RemoveEfMigration(string workingFolder, bool forceRemove = true)
        {
            var task = new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("migrations", "remove");

            if (forceRemove)
                task.WithArguments("--force");

            return task;
        }

        public static ExecuteDotnetTask EfUpdateDatabase(string workingFolder, string migrationName = "default")
        {
            return new ExecuteDotnetTask("ef")
                .WorkingFolder(workingFolder)
                .WithArguments("database", "update");
        }
    }
}
