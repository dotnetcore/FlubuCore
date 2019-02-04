using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCloneTask : ExternalProcessTaskBase<int, GitCloneTask>
    {
        private readonly string _repository;
        private readonly string _directory;
        private string _description;

        public GitCloneTask(string repository, string directory)
        {
            _repository = repository;
            _directory = directory;
            ExecutablePath = "git";
            InsertArgument(0, "clone");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'clone' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// checkout specified branch instead of the remote's HEAD.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GitCloneTask Branch(string name)
        {
            WithArgumentsValueRequired("--branch", name);
            return this;
        }

        /// <summary>
        /// force progress reporting.
        /// </summary>
        /// <returns></returns>
        public GitCloneTask ShowProgress()
        {
            WithArguments("--progress");
            return this;
        }

        /// <summary>
        /// be more verbose.
        /// </summary>
        /// <returns></returns>
        public GitCloneTask Verbose()
        {
            WithArguments("--verbose");
            return this;
        }

        /// <summary>
        /// be more quiet.
        /// </summary>
        /// <returns></returns>
        public GitCloneTask Quiet()
        {
            WithArguments("--quit");
            return this;
        }

        /// <summary>
        /// don't create a checkout
        /// </summary>
        /// <returns></returns>
        public GitCloneTask NoCheckout()
        {
            WithArguments("--no-checkout");
            return this;
        }

        /// <summary>
        /// create a bare repository.
        /// </summary>
        /// <returns></returns>
        public GitCloneTask CreateBareRepository()
        {
            WithArguments("--bare");
            return this;
        }

        /// <summary>
        /// setup as shared repository.
        /// </summary>
        /// <returns></returns>
        public GitCloneTask SetupAsSharedRepository()
        {
            WithArguments("--shared");
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _repository.MustNotBeNullOrEmpty("Url of the repository to clone must not be empty.");
            WithArguments(_directory, _repository);
            return base.DoExecute(context);
        }
    }
}
