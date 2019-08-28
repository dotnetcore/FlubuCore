using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Context
{
    public enum BuildSystemType
    {
        AppVeyor,
        Bamboo,
        Bitrise,
        ContinousCl,
        GitLabCi,
        GoCD,
        Jenkins,
        MyGet,
        TeamCity,
        TFS,
        TravisCI,
        AzurePipelines,
        LocalBuild
    }
}
