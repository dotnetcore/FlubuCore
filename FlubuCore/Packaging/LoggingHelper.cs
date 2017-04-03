using FlubuCore.Context;

namespace FlubuCore.Packaging
{
    public static class LoggingHelper
    {
        public static bool LogIfFilteredOut(string fileName, IFileFilter filter, ITaskContextInternal taskContext, bool logFiles)
        {
            if (filter != null && !filter.IsPassedThrough(fileName))
            {
                if (logFiles)
                {
                    taskContext.LogInfo(string.Format("File '{0}' has been filtered out.", fileName));
                }

                return false;
            }

            return true;
        }
    }
}