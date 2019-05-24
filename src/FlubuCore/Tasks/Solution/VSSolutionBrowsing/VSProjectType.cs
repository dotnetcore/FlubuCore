using System;

namespace FlubuCore.Tasks.Solution.VSSolutionBrowsing
{
    /// <summary>
    /// Contains information about a specific VisualStudio project type.
    /// </summary>
    public class VSProjectType : IEquatable<VSProjectType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VSProjectType"/> class.
        /// </summary>
        /// <param name="projectTypeGuid">The project type GUID.</param>
        /// <param name="projectTypeName">Name of the project type.</param>
        public VSProjectType(Guid projectTypeGuid, string projectTypeName)
        {
            ProjectTypeGuid = projectTypeGuid;
            ProjectTypeName = projectTypeName;
        }

        /// <summary>
        /// Gets the <see cref="VSProjectType"/> for C# projects.
        /// </summary>
        /// <value>The <see cref="VSProjectType"/> for C# projects.</value>
        public static VSProjectType CSharpProjectType { get; } = new VSProjectType(new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"), "C# Project");

        public static VSProjectType NewCSharpProjectType { get; } = new VSProjectType(new Guid("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}"), "New C# Project");

        /// <summary>
        /// Gets the <see cref="VSProjectType"/> for solution folders.
        /// </summary>
        /// <value>The <see cref="VSProjectType"/> for solution folders.</value>
        public static VSProjectType SolutionFolderProjectType { get; } = new VSProjectType(new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}"), "Solution Folder");

        /// <summary>
        /// Gets the project type GUID.
        /// </summary>
        /// <value>The project type GUID.</value>
        public Guid ProjectTypeGuid { get; }

        /// <summary>
        /// Gets the name of the project type.
        /// </summary>
        /// <value>The name of the project type.</value>
        public string ProjectTypeName { get; }

        /// <summary>
        /// Compares the two objects.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(VSProjectType left, VSProjectType right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares the two objects.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(VSProjectType left, VSProjectType right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        public bool Equals(VSProjectType other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return other.ProjectTypeGuid.Equals(ProjectTypeGuid);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != typeof(VSProjectType))
                return false;

            return Equals((VSProjectType)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return ProjectTypeGuid.GetHashCode();
        }
    }
}