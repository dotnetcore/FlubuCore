using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Templating.Models;

namespace FlubuCore.Templating.Tasks
{
    public interface IFlubuTemplateTask
    {
        void BeforeFileProcessing(TemplateModel template, List<string> files);

        void BeforeFileCopy(string sourceFilePath);

        void AfterFileCopy(string destinationFilePath);

        void AfterFileProcessing(TemplateModel template);
    }
}
