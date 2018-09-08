using FlubuCore.Tasks.Git;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IGitFluentInterface
    {
        /// <summary>
        /// Incorporates changes from a remote repository into the current branch.
        /// </summary>
        /// <returns></returns>
        GitPullTask Pull();

        /// <summary>
        /// Clones specified existring git repository into specified directory.
        /// </summary>
        /// <param name="repository">Url of the repository to clone.</param>
        /// <param name="directory">Directory where reposiotry will be cloned to.</param>
        /// <returns></returns>
        GitCloneTask Clone(string repository, string directory);

        /// <summary>
        /// This command updates the index using the current content found in the working tree, to prepare the content staged for the next commit.
        /// </summary>
        /// <returns></returns>
        GitAddTask Add();
    }
}
