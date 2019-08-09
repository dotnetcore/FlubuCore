using FlubuCore.Tasks.Attributes;
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
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("Submodule");
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
        [ArgKey("update")]
        public GitSubmoduleTask Update()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///     Add --init argument
        /// </summary>
        /// <returns></returns>
        [ArgKey("--init")]
        public GitSubmoduleTask Init()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///     Add --recursive argument
        /// </summary>
        /// <returns></returns>
        [ArgKey("--recursive")]
        public GitSubmoduleTask Recursive()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///     Add --remote argument
        /// </summary>
        /// <returns></returns>
        [ArgKey("--remote")]
        public GitSubmoduleTask Remote()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///     Add --merge argument.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--merge")]
        public GitSubmoduleTask Merge()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}