using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitSubmoduleTask : ExternalProcessTaskBase<int, GitSubmoduleTask>
    {
        private string _description;

        public GitSubmoduleTask()
        {
            ExecutablePath = "git";
            InsertArgument(0, "submodule");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                    return "Executes git command 'submodule' with specified option.";

                return _description;
            }

            set => _description = value;
        }

        /// <summary>
        ///     Submodule update command.
        /// </summary>
        /// <returns></returns>
        public GitSubmoduleTask Update()
        {
            WithArguments("update");
            return this;
        }

        /// <summary>
        ///     Add --init argument
        /// </summary>
        /// <returns></returns>
        public GitSubmoduleTask Init()
        {
            WithArguments("--init");
            return this;
        }

        /// <summary>
        ///     Add --recursive argument
        /// </summary>
        /// <returns></returns>
        public GitSubmoduleTask Recursive()
        {
            WithArguments("--recursive");
            return this;
        }

        /// <summary>
        ///     Add --remote argument
        /// </summary>
        /// <returns></returns>
        public GitSubmoduleTask Remote()
        {
            WithArguments("--remote");
            return this;
        }

        /// <summary>
        ///     Add --merge argument.
        /// </summary>
        /// <returns></returns>
        public GitSubmoduleTask Merge()
        {
            WithArguments("--merge");
            return this;
        }
    }
}