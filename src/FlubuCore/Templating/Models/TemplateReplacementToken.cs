using System.Collections.Generic;

namespace FlubuCore.Templating.Models
{
    public class TemplateReplacementToken
    {
        /// <summary>
        /// Token to be replaced in template files.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Input text shown in interactive mode when entering value for replacement token
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Help shown at the bottom of console in the interactive mode.
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Default value shown as hint in the beginning in interactive mode.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Input type of the replacement token.
        /// </summary>
        public InputType InputType { get; set; }

        /// <summary>
        /// Values for <see cref="InputType"/> Hint and Options.
        /// </summary>
        public List<string> Values { get; set; }

        /// <summary>
        /// Options for 'Files' InputType.
        /// </summary>
        public FilesInputType Files { get; set; }
    }
}
