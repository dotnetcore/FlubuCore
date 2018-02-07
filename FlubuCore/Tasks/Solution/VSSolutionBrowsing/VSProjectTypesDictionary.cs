using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// A dictionary of registered VisualStudio project types.
    /// </summary>
    public class VSProjectTypesDictionary
    {
        private readonly Dictionary<Guid, VSProjectType> _projectTypes = new Dictionary<Guid, VSProjectType>();

        public VSProjectTypesDictionary()
        {
            // add some common project types
            RegisterProjectType(VSProjectType.SolutionFolderProjectType);
            RegisterProjectType(VSProjectType.CSharpProjectType);
            RegisterProjectType(VSProjectType.NewCSharpProjectType);
        }

        /// <summary>
        /// Registers a new type of the VisualStudio project.
        /// </summary>
        /// <param name="projectType">><see cref="VSProjectType"/> object to be registered.</param>
        public void RegisterProjectType(VSProjectType projectType)
        {
            _projectTypes.Add(projectType.ProjectTypeGuid, projectType);
        }

        /// <summary>
        /// Tries to find <see cref="VSProjectType"/> object for a specific VisualStudio project type Guid.
        /// </summary>
        /// <param name="projectTypeGuid">The project type GUID.</param>
        /// <returns><see cref="VSProjectType"/> object holding information about the specified VisualStudio project
        /// type; <c>null</c> if the project type is not registered.</returns>
        public VSProjectType FindProjectType(Guid projectTypeGuid)
        {
            if (_projectTypes.ContainsKey(projectTypeGuid))
                return _projectTypes[projectTypeGuid];

            return null;
        }
    }
}