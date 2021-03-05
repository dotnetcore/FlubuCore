using System.Collections.Generic;
using FlubuCore.Templating.Models;

namespace FlubuCore.Templating
{
    public class FlubuTemplateBuilder : IFlubuTemplateBuilder
    {
        private List<TemplateReplacementToken> _replacementTokens = new List<TemplateReplacementToken>();

        private List<string> _filesToSkip = new List<string>();

        public FlubuTemplateBuilder AddReplacementToken(TemplateReplacementToken token)
        {
            _replacementTokens.Add(token);
            return this;
        }

        public FlubuTemplateBuilder AddFileToSkip(string file)
        {
            _filesToSkip = new List<string>();
            return this;
        }

        public TemplateModel Build()
        {
            return new TemplateModel
            {
                Tokens = _replacementTokens,
                SkipFiles = _filesToSkip
            };
        }
    }
}
