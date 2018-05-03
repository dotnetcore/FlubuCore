using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers;

namespace FlubuCore.Context
{
    public class BuildServers : IBuildServers
    {
        public bool IsLocalBuild => RunningOn == BuildServerType.LocalBuild;

        public BuildServerType RunningOn
        {
            get
            {
                if (AppVeyor.IsRunningOnAppVeyor)
                {
                    return BuildServerType.AppVeyor;
                }

                if (Bamboo.IsRunningOnBamboo)
                {
                    return BuildServerType.Bamboo;
                }

                if (BitRise.IsRunningOnBitrise)
                {
                    return BuildServerType.Bitrise;
                }

                if (ContinuaCl.IsRunningOnContinuaCl)
                {
                    return BuildServerType.ContinousCl;
                }

                if (GitLab.IsRunningOnGitLabCi)
                {
                    return BuildServerType.GitLabCi;
                }

                if (GoCD.IsRunningOnGoCD)
                {
                    return BuildServerType.GoCD;
                }

                if (Jenkins.IsRunningOnJenkins)
                {
                    return BuildServerType.Jenkins;
                }

                if (MyGet.IsRunningOnMyGet)
                {
                    return BuildServerType.MyGet;
                }

                if (TeamCity.IsRunningOnTeamCity)
                {
                    return BuildServerType.TeamCity;
                }

                if (TeamFoundation.IsRunningOnTFS)
                {
                    return BuildServerType.TFS;
                }

                if (Travis.IsRunningOnTravis)
                {
                    return BuildServerType.TravisCI;
                }

                return BuildServerType.LocalBuild;
            }
        }
    }
}
