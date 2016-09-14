using System.Globalization;
using flubu.Tasks;

namespace flubu
{
    public static class TaskContextExtensions
    {
        public static void Fail(
            this ITaskContext context, 
            string messageFormat, 
            params object[] args)
        {
            if (messageFormat == null)
                return;

            string message = string.Format(
                CultureInfo.InvariantCulture, 
                messageFormat, 
                args);
            context.Fail(message);
        }

        public static void WriteMessage(
            this ITaskContext context, 
            TaskMessageLevel level, 
            string messageFormat, 
            params object[] args)
        {
            if (messageFormat == null)
                return;

            string message = string.Format(
                CultureInfo.InvariantCulture, 
                messageFormat, 
                args);
            context.WriteMessage(level, message);
        }

        public static void WriteDebug(this ITaskContext context, string messageFormat, params object[] args)
        {
            context.WriteMessage(TaskMessageLevel.Debug, messageFormat, args);
        }

        public static void WriteError(this ITaskContext context, string messageFormat, params object[] args)
        {
            context.WriteMessage(TaskMessageLevel.Error, messageFormat, args);
        }

        public static void WriteInfo(this ITaskContext context, string messageFormat, params object[] args)
        {
            context.WriteMessage(TaskMessageLevel.Info, messageFormat, args);
        }


        //TODO implement
        //public static FullPath GetProjectOutputPath(this ITaskContext context, string projectName)
        //{
        //    VSSolution solution = context.Properties.Get<VSSolution>(BuildProps.Solution);
        //    VSProjectWithFileInfo projectInfo =
        //        (VSProjectWithFileInfo)solution.FindProjectByName(projectName);

        //    LocalPath projectOutputPath = projectInfo.GetProjectOutputPath(
        //        context.Properties.Get<string>(BuildProps.BuildConfiguration));
        //    FullPath projectTargetDir = projectInfo.ProjectDirectoryPath.CombineWith(projectOutputPath);
        //    return projectTargetDir;
        //}
    }
}