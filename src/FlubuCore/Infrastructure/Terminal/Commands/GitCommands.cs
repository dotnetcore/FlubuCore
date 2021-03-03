using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Git;

namespace FlubuCore.Infrastructure.Terminal.Commands
{
    public static class GitCommands
    {
        public static Dictionary<string, Type> SupportedExternalProcesses { get; } = new Dictionary<string, Type>()
        {
            { "git add", typeof(GitAddTask) },
            { "git clean", typeof(GitCleanTask) },
            { "git fetch", typeof(GitFetchTask) },
            { "git checkout", typeof(GitCheckoutTask) },
            { "git clone", typeof(GitCloneTask) },
            { "git commit", typeof(GitCommitTask) },
            { "git pull", typeof(GitPullTask) },
            { "git push", typeof(GitPushTask) },
            { "git tag", typeof(GitTagTask) },
            { "git branch", typeof(GitBranchTask) },
            { "git merge", typeof(GitMergeTask) },
            { "git submodule", typeof(GitSubmoduleTask) },
            { "git rm", typeof(GitRemoveFilesTask) }
        };

       public static KeyValuePair<string, IReadOnlyCollection<Hint>> GitCommandHints { get; } = new KeyValuePair<string, IReadOnlyCollection<Hint>>("git", new List<Hint>()
       {
           new Hint() { Name = "commit", Help = "Create a new commit containing the current contents of the index and the given log message describing the changes." },
           new Hint() { Name = "pull", Help = "Incorporates changes from a remote repository into the current branch. In its default mode." },
           new Hint() { Name = "add", Help = "This command updates the index using the current content found in the working tree, to prepare the content staged for the next commit." },
           new Hint() { Name = "checkout", Help = "Updates files in the working tree to match the version in the index or the specified tree. " },
           new Hint() { Name = "clone", Help = "Clones a repository into a newly created directory." },
           new Hint() { Name = "tag", Help = "Add a tag reference in refs/tags/, unless -d/-l/-v is given to delete, list or verify tags." },
           new Hint() { Name = "submodule", Help = "Initialize, update or inspect submodules." },
           new Hint() { Name = "rm", Help = "Remove files from the index, or from the working tree and the index." },
           new Hint() { Name = "merge", Help = "Join two or more development histories together." },
           new Hint() { Name = "branch", Help = "List, create, or delete branches." },
           new Hint() { Name = "push", Help = "Updates remote refs using local refs, while sending objects necessary to complete the given refs." },
       });
    }
}
