using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCheckoutTask : ExternalProcessTaskBase<int, GitCheckoutTask>
    {
        public GitCheckoutTask(string branch)
        {
            ExecutablePath = "git";
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            WithArguments("checkout", branch);
            AddAdditionalOptionPrefix("Checkout");
        }

        protected override string Description
        {
            get => "Executes git command 'checkout' with specified option.";
            set { }
        }

        /// <summary>
        /// Detach repo.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--detach")]
        public GitCheckoutTask Detach()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}
