namespace FlubuCore.Templating
{
    public interface IFlubuTemplateBuilder
    {
        FlubuTemplateBuilder AddReplacementToken(TemplateReplacmentToken token);

        TemplateModel Build();
    }
}
