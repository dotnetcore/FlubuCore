using System;
using System.Collections.Generic;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    public class VSSolutionFilesInfo : VSProjectInfo
    {
        private readonly List<string> _files = new List<string>();

        public VSSolutionFilesInfo(
            VSSolution ownerSolution,
            Guid projectGuid,
            string projectName,
            Guid projectTypeGuid)
            : base(ownerSolution, projectGuid, projectName, projectTypeGuid)
        {
        }

        public IList<string> Files => _files;

        public override void Parse(VSSolutionFileParser parser)
        {
            string line = parser.NextLine().Trim();
            if (line == "EndProject")
                return;

            if (line != "ProjectSection(SolutionItems) = preProject")
                parser.ThrowParserException("Unexpected token. 'ProjectSection' expected.");

            while (true)
            {
                line = parser.NextLine().Trim();
                if (line == "EndProjectSection")
                    break;

                string[] splits = line.Split('=');
                if (splits.Length != 2)
                    parser.ThrowParserException("Unexpected token.");

                _files.Add(splits[0].Trim());
            }

            line = parser.NextLine().Trim();
            if (line != "EndProject")
                parser.ThrowParserException("'EndProject' expected.");
        }
    }
}