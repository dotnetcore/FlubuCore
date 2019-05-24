using System;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    public abstract class VSProjectInfo
    {
        protected VSProjectInfo(
            VSSolution ownerSolution,
            Guid projectGuid,
            string projectName,
            Guid projectTypeGuid)
        {
            OwnerSolution = ownerSolution;
            ProjectTypeGuid = projectTypeGuid;
            ProjectName = projectName;
            ProjectGuid = projectGuid;
        }

        public VSSolution OwnerSolution { get; }

        public Guid ProjectGuid { get; }

        public string ProjectName { get; }

        public Guid ProjectTypeGuid { get; }

        public abstract void Parse(VSSolutionFileParser parser);
    }
}
