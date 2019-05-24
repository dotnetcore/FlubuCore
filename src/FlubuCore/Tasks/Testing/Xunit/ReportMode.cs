using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Testing.Xunit
{
    public enum ReportMode
    {
        /// <summary>
        /// forces AppVeyor CI mode (normally auto-detected)
        /// </summary>
        AppVeyor,

        /// <summary>
        /// show progress messages in JSON format
        /// </summary>
        Json,

        /// <summary>
        /// do not show progress messages
        /// </summary>
        Quiet,

        /// <summary>
        /// forces TeamCity mode (normally auto-detected)
        /// </summary>
        TeamCity,

      /// <summary>
      /// show verbose progress messages
      /// </summary>
        Verbose,

        /// <summary>
        /// Auto detect.
        /// </summary>
        None,
    }
}
