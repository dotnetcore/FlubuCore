using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context.BuildServers;

namespace FlubuCore.Context
{
    public interface IBuildSystem
    {
        /// <summary>
        /// If <c>true</c> build is local. Otherwise it is running on known build server. See <see cref="BuildSystemType"/> for known build servers.
        /// </summary>
        bool IsLocalBuild { get; }

        /// <summary>
        /// Gets a value on which type of build server the build is running.
        /// </summary>
        BuildSystemType RunningOn { get; }

        AppVeyor AppVeyor();

        Bamboo Bamboo();

        BitRise BitRise();

        ContinuaCl ContinuaCl();

        GoCD GoCd();

        Jenkins Jenkins();

        MyGet MyGet();

        TeamCity TeamCity();

        TeamFoundation TeamFoundationServer();

        Travis Travis();

        AzurePipelines AzurePipelines();
    }
}
