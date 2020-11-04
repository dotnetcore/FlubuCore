using System.Collections.Generic;
using FlubuCore.Templating.Models;

namespace FlubuCore.Templating
{
    public class FlubuTemplateBuilder : IFlubuTemplateBuilder
    {
        private List<TemplateReplacementToken> _replacementTokens = new List<TemplateReplacementToken>();

        public FlubuTemplateBuilder AddReplacementToken(TemplateReplacementToken token)
        {
            _replacementTokens.Add(token);
            return this;
        }

        public TemplateModel Build()
        {
            return new TemplateModel
            {
                Tokens = _replacementTokens
            };
        }
    }
}
