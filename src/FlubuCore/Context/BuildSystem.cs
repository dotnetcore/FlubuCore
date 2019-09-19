using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context.BuildServers;

namespace FlubuCore.Context
{
    public class BuildSystem : IBuildSystem
    {
        public bool IsLocalBuild => RunningOn == BuildSystemType.LocalBuild;

        public BuildSystemType RunningOn
        {
            get
            {
                if (BuildServers.AppVeyor.RunningOnAppVeyor)
                {
                    return BuildSystemType.AppVeyor;
                }

                if (BuildServers.Bamboo.RunningOnBamboo)
                {
                    return BuildSystemType.Bamboo;
                }

                if (BuildServers.BitRise.RunningOnBitrise)
                {
                    return BuildSystemType.Bitrise;
                }

                if (BuildServers.ContinuaCl.RunningOnContinuaCl)
                {
                    return BuildSystemType.ContinousCl;
                }

                if (BuildServers.GitLab.RunningOnGitLabCi)
                {
                    return BuildSystemType.GitLabCi;
                }

                if (BuildServers.GoCD.RunningOnGoCD)
                {
                    return BuildSystemType.GoCD;
                }

                if (BuildServers.Jenkins.RunningOnJenkins)
                {
                    return BuildSystemType.Jenkins;
                }

                if (BuildServers.MyGet.RunningOnMyGet)
                {
                    return BuildSystemType.MyGet;
                }

                if (BuildServers.TeamCity.RunningOnTeamCity)
                {
                    return BuildSystemType.TeamCity;
                }

                if (TeamFoundation.RunningOnTFS)
                {
                    return BuildSystemType.TFS;
                }

                if (BuildServers.Travis.RunningOnTravis)
                {
                    return BuildSystemType.TravisCI;
                }

                if (BuildServers.AzurePipelines.RunningOnAzurePipelines || BuildServers.AzurePipelines.RunningOnAzurePipelinesHosted)
                {
                    return BuildSystemType.AzurePipelines;
                }

                if (BuildServers.GitHubActions.RunningOnGitHubActions)
                {
                    return BuildSystemType.GitHubActions;
                }

                return BuildSystemType.LocalBuild;
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
