using FlubuCore.Templating.Models;

namespace FlubuCore.Templating
{
    public interface IFlubuTemplateBuilder
    {
        /// <summary>
        /// Adds Replacement token to replace in template files.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        FlubuTemplateBuilder AddReplacementToken(TemplateReplacementToken token);

        /// <summary>
        /// Added files are not copied from template to the newly created project with command 'flubu new'.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        FlubuTemplateBuilder AddFileToSkip(string file);

        TemplateModel Build();
    }
}
