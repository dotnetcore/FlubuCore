using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations.Models.Jenkins
{
    public static class StageExtensions
    {
        /// <summary>
        /// Adds step to a stage. Steps defines a series of one or more steps to be executed in a given stage directive.
        /// </summary>
        /// <param name="stage"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Stage AddStep(this Stage stage, string step)
        {
            stage.Steps.Add(step);
            return stage;
        }
    }
}
