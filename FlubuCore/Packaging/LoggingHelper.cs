using FlubuCore.Context;

namespace FlubuCore.Packaging
{
    public static class LoggingHelper
    {
        public static bool LogIfFilteredOut(string fileName, IFileFilter filter, ITaskContext taskContext)
        {
            if (filter != null && !filter.IsPassedThrough(fileName))
            {
                taskContext.LogInfo(string.Format("File '{0}' has been filtered out.", fileName));
                return false;
            }

            return true;
        }
    }
}