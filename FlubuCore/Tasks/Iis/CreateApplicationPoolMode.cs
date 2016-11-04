using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public enum CreateApplicationPoolMode
    {
        /// <summary>
        /// The task should fail if the application pool already exists.
        /// </summary>
        FailIfAlreadyExists,

        /// <summary>
        /// If the application pool already exists, it should be updated.
        /// </summary>
        UpdateIfExists,

        /// <summary>
        /// If the application pool already exists, the task should do nothing.
        /// </summary>
        DoNothingIfExists,
    }
}
