using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Tasks.Git;

namespace FlubuCore.Context.FluentInterface
{
    public class GitFluentInterface : IGitFluentInterface
    {
        public TaskContext Context { get; set; }

        public GitAddTask Add()
        {
             return Context.CreateTask<GitAddTask>();
        }

        public GitCloneTask Clone(string repository, string directory)
        {
            return Context.CreateTask<GitCloneTask>(repository, directory);
        }

        public GitPullTask Pull()
        {
             return Context.CreateTask<GitPullTask>();
        }
    }
}
