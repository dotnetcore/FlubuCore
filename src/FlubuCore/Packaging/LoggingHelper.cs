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
                    taskContext.LogInfo($"File '{fileName}' has been filtered out.");
                }

                return false;
            }

            return true;
        }
    }
}