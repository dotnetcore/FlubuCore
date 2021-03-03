using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Git;

namespace FlubuCore.Context.FluentInterface
{
    public class GitFluentInterface : IGitFluentInterface
    {
        public TaskContext Context { get; set; }

        public GitCheckoutTask Checkout(string branch)
        {
            return new GitCheckoutTask(branch);
        }

        public GitAddTask Add()
        {
             return Context.CreateTask<GitAddTask>();
        }

        public GitCleanTask Clean()
        {
            return Context.CreateTask<GitCleanTask>();
        }

        public GitFetchTask Fetch()
        {
            return Context.CreateTask<GitFetchTask>();
        }

        public GitCloneTask Clone(string repository, string directory)
        {
            return Context.CreateTask<GitCloneTask>(repository, directory);
        }

        public GitCommitTask Commit()
        {
           return Context.CreateTask<GitCommitTask>();
        }

        public GitPullTask Pull()
        {
             return Context.CreateTask<GitPullTask>();
        }

        public GitPushTask Push()
        {
             return Context.CreateTask<GitPushTask>();
        }

        public GitMergeTask GitMerge()
        {
            return Context.CreateTask<GitMergeTask>();
        }

        public GitRemoveFilesTask RemoveFile(string file)
        {
             return Context.CreateTask<GitRemoveFilesTask>(file);
        }

        public GitTagTask Tag(string name)
        {
            return Context.CreateTask<GitTagTask>(name);
        }

        public GitSubmoduleTask Submodule()
        {
            return Context.CreateTask<GitSubmoduleTask>();
        }

        public GitSubmoduleTask InitSubmodules()
        {
            return Submodule().Update().Init().Recursive();
        }

        public GitSubmoduleTask PullSubmodules()
        {
            return Submodule().Update().Remote().Merge();
        }

        public GitBranchTask GitBranch()
        {
            return Context.CreateTask<GitBranchTask>();
        }
    }
}
