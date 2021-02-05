using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FlubuCore.Infrastructure;
using YamlDotNet.Serialization;

namespace FlubuCore.BuildServers.Configurations.Models.Travis
{
    public class TravisConfiguration
    {
        public string Language { get; set; }

        public List<string> Os { get; set; }

        [YamlMember(Alias = "osx_image")]
        public string OSXImage { get; set; }

        public string Sudo { get; set; }

        public string Dist { get; set; }

        public string Dotnet { get; set; }

        public string Mono { get; set; }

        public Branches Branches { get; set; }

        public List<string> Env { get; set; }

        public List<string> Services { get; set; }

        public List<string> Install { get; set; }

        [YamlMember(Alias = "before_script", ApplyNamingConventions = false)]
        public List<string> BeforeScript { get; set; }

        public List<string> Script { get; set; }

        public void FromOptions(TravisOptions options)
        {
            Language = options.Language;
            OSXImage = options.OsImage;
            Sudo = options.Sudo;
            Dist = options.Dist;
            Dotnet = options.DotnetVersion;
            Mono = options.Mono;

            if (!options.Os.IsNullOrEmpty())
            {
                Os = new List<string>();
                foreach (var os in options.Os)
                {
                    switch (os)
                    {
                        case TravisOs.Linux:
                            Os.Add("linux");
                            break;
                        case TravisOs.Windows:
                            Os.Add("windows");
                            break;
                        case TravisOs.MacOS:
                            Os.Add("osx");
                            break;
                    }
                }
            }

            if (options.Scripts.IsNullOrEmpty())
            {
                Script = options.Scripts;
            }

            Script = options.Scripts;

            if (!options.Services.IsNullOrEmpty())
            {
                Services = options.Services;
            }

            if (!options.BeforeScript.IsNullOrEmpty())
            {
                BeforeScript = options.BeforeScript;
            }

            if (!options.ScriptsAfter.IsNullOrEmpty())
            {
                Script.AddRange(options.ScriptsAfter);
            }

            if (!options.Install.IsNullOrEmpty())
            {
                Install = options.Install;
            }

            if (!options.EnvironemtVariables.IsNullOrEmpty())
            {
                Env = options.EnvironemtVariables;
            }

            if (!options.BranchesOnly.IsNullOrEmpty())
            {
                Branches = new Branches()
                {
                    Only = options.BranchesOnly
                };
            }
        }
    }
}
