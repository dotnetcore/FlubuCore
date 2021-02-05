using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.GitHubActions
{
    public class GitHubActionJob
    {
        private List<object> _steps = new List<object>();

        public string Name { get; set; }

        [YamlMember(Alias = "runs-on", ApplyNamingConventions = false)]
        public string RunsOn { get; set; }

        public Dictionary<string, string> Env { get; set; }

        public IList<object> Steps => _steps.AsReadOnly();

        public void AddStep<T>(T step)
            where T : IGitHubActionStep
        {
            _steps.Add(step);
        }
    }
}
