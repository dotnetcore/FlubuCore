using System.Collections.Generic;

namespace FlubuCore.Templating
{
    public class FlubuTemplateBuilder : IFlubuTemplateBuilder
    {
        private List<TemplateReplacmentToken> _replacementTokens = new List<TemplateReplacmentToken>();

        public FlubuTemplateBuilder AddReplacementToken(TemplateReplacmentToken token)
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
