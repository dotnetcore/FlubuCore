using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlubuCore.Tasks.Iis
{
    public enum ControlApplicationPoolAction
    {
        /// <summary>
        /// Start the application pool.
        /// </summary>
        Start,

        /// <summary>
        /// Stop the application pool.
        /// </summary>
        Stop,

        /// <summary>
        /// Recycle the application pool.
        /// </summary>
        Recycle,
    }
}
