using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Solution
{
    public enum StandardConsoleLoggerParameters
    {
        /// <summary>
        /// Show time spent in tasks, targets and projects.
        /// </summary>
        PerformanceSummary,

        /// <summary>
        /// Show error and warning summary at the end.
        /// </summary>
        Summary,

        /// <summary>
        /// Don't show error and warning summary at the end.
        /// </summary>
        NoSummary,

        /// <summary>
        /// Show only errors.
        /// </summary>
        ErrorsOnly,

        /// <summary>
        /// Show only warnings.
        /// </summary>
        WarningsOnly,

        /// <summary>
        /// Don't show list of items and properties at the start of each project build.
        /// </summary>
        NoItemAndPropertyList,

        /// <summary>
        /// Show TaskCommandLineEvent messages
        /// </summary>
        ShowCommandLine,

        /// <summary>
        /// Display the Timestamp as a prefix to any message.
        /// </summary>
        ShowTimestamp,

        /// <summary>
        /// Show eventId for started events, finished events, and messages
        /// </summary>
        ShowEventId,

        /// <summary>
        /// Does not align the text to the size of the console buffer
        /// </summary>
        ForceNoAlign,

        /// <summary>
        /// Use the default console colors for all logging messages.
        /// </summary>
        DisableConsoleColor,

        /// <summary>
        /// Disable the multiprocessor logging style of output when running in non-multiprocessor mode.
        /// </summary>
        DisableMPLogging,

        /// <summary>
        /// Enable the multiprocessor logging style even when running in non-multiprocessor mode. This logging style is on by default.
        /// </summary>
        EnableMPLogging,

        /// <summary>
        /// Use ANSI console colors even if console does not support it
        /// </summary>
        ForceConsoleColor,
    }
}
