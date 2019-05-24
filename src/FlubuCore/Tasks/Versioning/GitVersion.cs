using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.Versioning
{
    public class GitVersion
    {
        public virtual int Major { get; internal set; }

        public virtual int Minor { get; internal set; }

        public virtual int Patch { get; internal set; }

        public virtual string PreReleaseTag { get; internal set; }

        public virtual string PreReleaseTagWithDash { get; internal set; }

        public virtual string PreReleaseLabel { get; internal set; }

        public virtual string PreReleaseNumber { get; internal set; }

        public virtual string BuildMetaData { get; internal set; }

        public virtual string BuildMetaDataPadded { get; internal set; }

        public virtual string FullBuildMetaData { get; internal set; }

        public virtual string MajorMinorPatch { get; internal set; }

        public virtual string SemVer { get; internal set; }

        public virtual string LegacySemVer { get; internal set; }

        public virtual string LegacySemVerPadded { get; internal set; }

        public virtual string AssemblySemVer { get; internal set; }

        public virtual string FullSemVer { get; internal set; }

        public virtual string InformationalVersion { get; internal set; }

        public virtual string BranchName { get; internal set; }

        public virtual string Sha { get; internal set; }

        public virtual string NuGetVersionV2 { get; internal set; }

        public virtual string NuGetVersion { get; internal set; }

        public virtual string NuGetPreReleaseTagV2 { get; internal set; }

        public virtual string NuGetPreReleaseTag { get; internal set; }

        public virtual string CommitsSinceVersionSource { get; internal set; }

        public virtual string CommitsSinceVersionSourcePadded { get; internal set; }

        public virtual string CommitDate { get; internal set; }
    }
}
