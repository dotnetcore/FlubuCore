using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Templating.Tasks
{
    public interface IFlubuTemplateTaskFactory
    {
        IFlubuTemplateTask GetFlubuTemplateTask(string taskName);
    }
}
