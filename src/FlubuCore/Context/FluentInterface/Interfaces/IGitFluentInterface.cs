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
        /// Stores the current contents of the index in a new commit along with a log message from the user describing the changes.
        /// </summary>
        /// <returns></returns>
        GitCommitTask Commit();

        /// <summary>
        /// Updates remote refs using local refs, while sending objects necessary to complete the given refs.
        /// </summary>
        /// <returns></returns>
        GitPushTask Push();

        /// <summary>
        /// Task removes untracked files from the working tree.
        /// </summary>
        /// <returns></returns>
        GitCleanTask Clean();

        /// <summary>
        /// Download objects and refs from remote repository
        /// </summary>
        /// <returns></returns>
        GitFetchTask Fetch();

        /// <summary>
        /// Clones specified existring git repository into specified directory.
        /// </summary>
        /// <param name="repository">Url of the repository to clone.</param>
        /// <param name="directory">Directory where reposiotry will be cloned to.</param>
        /// <returns></returns>
        GitCloneTask Clone(string repository, string directory);

        /// <summary>
        /// Checkout specified branch.
        /// </summary>
        /// <param name="branch">Branch to checkout.</param>
        /// <returns></returns>
        GitCheckoutTask Checkout(string branch);

        /// <summary>
        /// This command updates the index using the current content found in the working tree, to prepare the content staged for the next commit.
        /// </summary>
        /// <returns></returns>
        GitAddTask Add();

        /// <summary>
        /// Add a tag reference in refs/tags/, unless -d/-l/-v is given to delete, list or verify tags.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GitTagTask Tag(string name);

        /// <summary>
        /// git-branch - List, create, or delete branches.
        /// </summary>
        /// <returns></returns>
        GitBranchTask GitBranch();

        /// <summary>
        /// Join two or more development histories together.
        /// </summary>
        /// <returns></returns>
        GitMergeTask GitMerge();

        /// <summary>
        /// Remove files from the index, or from the working tree and the index. git rm will not remove a file from just your working directory.
        /// </summary>
        /// <param name="file">Files to remove. Fileglobs (e.g. *.c) can be given to remove all matching files.</param>
        /// <returns></returns>
        GitRemoveFilesTask RemoveFile(string file);

        /// <summary>
        /// Create raw submodule GIT command.
        /// </summary>
        /// <returns></returns>
        GitSubmoduleTask Submodule();

        /// <summary>
        /// Create submodules init task with --init and --recursive arguments.
        /// </summary>
        /// <returns></returns>
        GitSubmoduleTask InitSubmodules();

        /// <summary>
        /// Create submodules pull task with --remote and --merge arguments.
        /// </summary>
        /// <returns></returns>
        GitSubmoduleTask PullSubmodules();
    }
}
