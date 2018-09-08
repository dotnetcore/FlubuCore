using FlubuCore.Tasks.Git;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IGitFluentInterface
    {
        /// <summary>
        /// Clones specified existring git repository into specified directory.
        /// </summary>
        /// <param name="repository">Url of the repository to clone.</param>
        /// <param name="directory">Directory where reposiotry will be cloned to.</param>
        /// <returns></returns>
        GitCloneTask Clone(string repository, string directory);
    }
}
