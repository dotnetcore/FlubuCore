using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using FlubuCore.IO;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// Represents a VisualStudio solution.
    /// </summary>
    public class VSSolution
    {
        public static readonly Regex RegexEndProject = new Regex(@"^EndProject$", RegexOptions.Compiled);

        public static readonly Regex RegexGlobal = new Regex(@"^Global$", RegexOptions.Compiled);

        public static readonly Regex RegexSolutionProperty = new Regex(@"^(?<name>.*) = (?<value>.*)$", RegexOptions.Compiled);

        public static readonly Regex RegexProject = new Regex(@"^Project\(""(?<projectTypeGuid>.*)""\) = ""(?<name>.*)"", ""(?<path>.*)"", ""(?<projectGuid>.*)""$", RegexOptions.Compiled);

        public static readonly Regex RegexSolutionVersion = new Regex(@"^Microsoft Visual Studio Solution File, Format Version (?<version>.+)$", RegexOptions.Compiled);

        private readonly List<VSProjectInfo> _projects = new List<VSProjectInfo>();

        protected VSSolution(string fileName)
        {
            SolutionFileName = new FileFullPath(fileName);
        }

        /// <summary>
        /// Gets a read-only collection of <see cref="VSProjectWithFileInfo"/> objects for all of the projects in the solution.
        /// </summary>
        /// <value>A read-only collection of <see cref="VSProjectWithFileInfo"/> objects .</value>
        public ReadOnlyCollection<VSProjectInfo> Projects => _projects.AsReadOnly();

        public FullPath SolutionDirectoryPath => SolutionFileName.Directory;

        public FileFullPath SolutionFileName { get; }

        public decimal SolutionVersion { get; private set; }

        /// <summary>
        /// Loads the specified VisualStudio solution file and returns a <see cref="VSSolution"/> representing the solution.
        /// </summary>
        /// <param name="fileName">The name of the solution file.</param>
        /// <returns>A <see cref="VSSolution"/> representing the solution.</returns>
        public static VSSolution Load(string fileName)
        {
            VSSolution solution = new VSSolution(fileName);

            using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    VSSolutionFileParser parser = new VSSolutionFileParser(reader);

                    string line = parser.NextLine();

                    Match solutionMatch = RegexSolutionVersion.Match(line);

                    if (solutionMatch.Success == false)
                        parser.ThrowParserException("Not a solution file.");

                    solution.SolutionVersion = decimal.Parse(
                        solutionMatch.Groups["version"].Value,
                        CultureInfo.InvariantCulture);

                    while (true)
                    {
                        line = parser.NextLine();

                        if (line == null)
                            break;

                        // exit the loop when 'Global' section appears
                        if (RegexGlobal.IsMatch(line))
                            break;

                        Match projectMatch = RegexProject.Match(line);

                        if (!projectMatch.Success)
                        {
                            // skip any solution properties
                            if (RegexSolutionProperty.IsMatch(line))
                                continue;

                            // if nothing matches, we have a problem
                            parser.ThrowParserException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Could not parse solution file (line {0}): '{1}'.",
                                    parser.LineCount,
                                    line));
                        }

                        Guid projectGuid = new Guid(projectMatch.Groups["projectGuid"].Value);
                        string projectName = projectMatch.Groups["name"].Value;
                        string projectFileName = projectMatch.Groups["path"].Value;
                        Guid projectTypeGuid = new Guid(projectMatch.Groups["projectTypeGuid"].Value);

                        VSProjectInfo project;
                        if (projectTypeGuid == VSProjectType.SolutionFolderProjectType.ProjectTypeGuid)
                        {
                            project = new VSSolutionFilesInfo(
                                solution,
                                projectGuid,
                                projectName,
                                projectTypeGuid);
                        }
                        else
                        {
                            project = new VSProjectWithFileInfo(
                                solution,
                                projectGuid,
                                projectName,
                                new LocalPath(projectFileName),
                                projectTypeGuid);
                        }

                        solution._projects.Add(project);
                        project.Parse(parser);
                    }
                }
            }

            return solution;
        }

        public VSProjectInfo FindProjectByName(string projectName)
        {
            foreach (VSProjectInfo projectData in _projects)
            {
                if (projectData.ProjectName == projectName)
                    return projectData;
            }

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Project {0} not found.", projectName));
        }

        /// <summary>
        /// Performs the specified action on each project of the solution.
        /// </summary>
        /// <param name="action">The action delegate to perform on each project.</param>
        public void ForEachProject(Action<VSProjectInfo> action)
        {
            _projects.ForEach(action);
        }

        /// <summary>
        /// Loads the VisualStudio project files and fills the project data into <see cref="VSProjectWithFileInfo.Project"/>
        /// properties for each of the project in the solution.
        /// </summary>
        public void LoadProjects()
        {
            ForEachProject(projectInfo =>
            {
                if (projectInfo.ProjectTypeGuid == VSProjectType.CSharpProjectType.ProjectTypeGuid || projectInfo.ProjectTypeGuid == VSProjectType.NewCSharpProjectType.ProjectTypeGuid)
                    ((VSProjectWithFileInfo)projectInfo).Project = VSProject.Load(((VSProjectWithFileInfo)projectInfo).ProjectFileNameFull.ToString());
            });
        }
    }
}