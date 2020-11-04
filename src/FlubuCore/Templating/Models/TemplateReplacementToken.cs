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

        public string DefaultValue { get; set; }

        public InputType? InputType { get; set; }

        /// <summary>
        /// Options for 'Files' InputType.
        /// </summary>
        public FilesInputType Files { get; set; }
    }
}
