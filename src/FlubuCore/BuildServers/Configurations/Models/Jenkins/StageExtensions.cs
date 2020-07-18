using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public static class StageExtensions
    {
        public static Stage AddStep(this Stage stage, string step)
        {
            stage.Steps.Add(step);
            return stage;
        }
    }
}
