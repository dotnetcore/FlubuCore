using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    public interface IBuildServers
    {
        /// <summary>
        /// If <c>true</c> build is local. Otherwise it is running on known build server. See <see cref="BuildServerType"/> for known build servers.
        /// </summary>
        bool IsLocalBuild { get; }

        /// <summary>
        /// Gets a value on which type of build server the build is running.
        /// </summary>
        BuildServerType RunningOn { get; }
    }
}
