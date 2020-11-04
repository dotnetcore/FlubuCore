using FlubuCore.Templating.Models;

namespace FlubuCore.Templating
{
    public interface IFlubuTemplateBuilder
    {
        FlubuTemplateBuilder AddReplacementToken(TemplateReplacementToken token);

        TemplateModel Build();
    }
}
