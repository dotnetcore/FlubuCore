using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public enum CreateWebApplicationMode
    {
        /// <summary>
        /// If the application already exists, the task should fail.
        /// </summary>
        FailIfAlreadyExists,

        /// <summary>
        /// If the application already exists, it should be updated.
        /// </summary>
        UpdateIfExists,

        /// <summary>
        /// If the application already exists, the task should do nothing.
        /// </summary>
        DoNothingIfExists,
    }
}
