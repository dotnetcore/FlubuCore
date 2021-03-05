using System.Collections.Generic;

namespace FlubuCore.Templating.Models
{
    public class TemplateModel
    {
        public List<TemplateReplacementToken> Tokens { get; set; }

        public List<string> SkipFiles { get; set; } = new List<string>();
    }
}
