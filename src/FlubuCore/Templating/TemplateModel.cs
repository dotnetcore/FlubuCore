using System.Collections.Generic;

namespace FlubuCore.Templating
{
    public class TemplateModel
    {
        public List<TemplateReplacmentToken> Tokens { get; set; }

        public List<string> SkipFiles { get; set; }
    }
}
