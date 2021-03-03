using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Git
{
    public class GitFetchTask : ExternalProcessTaskBase<int, GitFetchTask>
    {
        private string _description;

        /// <summary>
        /// Download objects and refs from the repository
        /// </summary>
        public GitFetchTask()
        {
            ExecutablePath = "git";
            InsertArgument(0, "fetch");
            AddPrefixToAdditionalOptionKey(PrefixProcessors.AddDoubleDashPrefixToAdditionalOptionKey);
            AddAdditionalOptionPrefix("Fetch");
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes git command 'fetch' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Fetch all remotes.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--all")]
        public GitFetchTask All()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Fetch all tags from the remote (i.e., fetch remote tags refs/tags/* into local tags with the same name),
        /// in addition to whatever else would otherwise be fetched.
        /// Using this option alone does not subject tags to pruning, even if Prune is used (though tags may be pruned anyway if they are also the destination of an explicit refspec; see Prune).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--tags", "-t")]
        public GitFetchTask WithTags()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// By default, tags that point at objects that are downloaded from the remote repository are fetched and stored locally.
        /// This option disables this automatic tag following.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-tags", "-n")]
        public GitFetchTask NoTags()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Before fetching, remove any remote-tracking references that no longer exist on the remote.
        /// Tags are not subject to pruning if they are fetched only because of the default tag auto-following or due to a Tags option.
        /// However, if tags are fetched due to an explicit refspec (either on the command line or in the remote configuration, for example if the remote was cloned with the --mirror option), then they are also subject to pruning.
        /// Supplying PruneTags is a shorthand for providing the tag refspec.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--prune", "-p")]
        public GitFetchTask Prune()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Before fetching, remove any local tags that no longer exist on the remote if Prune is enabled.
        /// This option should be used more carefully, unlike Prune it will remove any local references (local tags) that have been created.
        /// This option is a shorthand for providing the explicit tag refspec along with Prune, see the discussion about that in its documentation.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--prune-tags", "-P")]
        public GitFetchTask PruneTags()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Force updating local refs.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force", "-f")]
        public GitFetchTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}
