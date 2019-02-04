using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitCheckoutTask : ExternalProcessTaskBase<int, GitCheckoutTask>
    {
        public GitCheckoutTask(string branch)
        {
            ExecutablePath = "git";
            WithArguments("checkout", branch);
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
        public GitCheckoutTask Detach()
        {
            WithArguments("--detach");
            return this;
        }
    }
}
