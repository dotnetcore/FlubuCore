using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.BuildServers;

namespace FlubuCore.Context
{
    public class BuildServer : IBuildServer
    {
        public bool IsLocalBuild => RunningOn == BuildServerType.LocalBuild;

        public BuildServerType RunningOn
        {
            get
            {
                if (BuildServers.AppVeyor.RunningOnAppVeyor)
                {
                    return BuildServerType.AppVeyor;
                }

                if (BuildServers.Bamboo.RunningOnBamboo)
                {
                    return BuildServerType.Bamboo;
                }

                if (BuildServers.BitRise.RunningOnBitrise)
                {
                    return BuildServerType.Bitrise;
                }

                if (BuildServers.ContinuaCl.RunningOnContinuaCl)
                {
                    return BuildServerType.ContinousCl;
                }

                if (BuildServers.GitLab.RunningOnGitLabCi)
                {
                    return BuildServerType.GitLabCi;
                }

                if (BuildServers.GoCD.RunningOnGoCD)
                {
                    return BuildServerType.GoCD;
                }

                if (BuildServers.Jenkins.RunningOnJenkins)
                {
                    return BuildServerType.Jenkins;
                }

                if (BuildServers.MyGet.RunningOnMyGet)
                {
                    return BuildServerType.MyGet;
                }

                if (BuildServers.TeamCity.RunningOnTeamCity)
                {
                    return BuildServerType.TeamCity;
                }

                if (TeamFoundation.RunningOnTFS)
                {
                    return BuildServerType.TFS;
                }

                if (BuildServers.Travis.RunningOnTravis)
                {
                    return BuildServerType.TravisCI;
                }

                if (BuildServers.AzurePipelines.RunningOnAzurePipelines || BuildServers.AzurePipelines.RunningOnAzurePipelinesHosted)
                {
                    return BuildServerType.AzurePipelines;
                }

                if (BuildServers.GitHubActions.RunningOnGitHubActions)
                {
                    return BuildServerType.GitHubActions;
                }

                return BuildServerType.LocalBuild;
            }
        }

        public AppVeyor AppVeyor()
        {
            return new AppVeyor();
        }

        public Bamboo Bamboo()
        {
            return new Bamboo();
        }

        public BitRise BitRise()
        {
            return new BitRise();
        }

        public ContinuaCl ContinuaCl()
        {
            return new ContinuaCl();
        }

        public GitLab GitLab()
        {
            return new GitLab();
        }

        public GoCD GoCd()
        {
            return new GoCD();
        }

        public Jenkins Jenkins()
        {
            return new Jenkins();
        }

        public MyGet MyGet()
        {
            return new MyGet();
        }

        public TeamCity TeamCity()
        {
            return new TeamCity();
        }

        public TeamFoundation TeamFoundationServer()
        {
            return new TeamFoundation();
        }

        public Travis Travis()
        {
            return new Travis();
        }

        public AzurePipelines AzurePipelines()
        {
            return new AzurePipelines();
        }

        public GitHubActions GitHubActions()
        {
            return new GitHubActions();
        }
    }
}
