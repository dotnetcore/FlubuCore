using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.BuildServers.Configurations
{
    public class TravisOptions
    {
        protected internal List<string> BeforeScript { get; set; } = new List<string>();

        protected internal List<TravisOs> Os { get; set; } = new List<TravisOs>();

        protected internal List<string> EnvironemtVariables { get; set; } = new List<string>();

        protected internal List<string> Services { get; set; } = new List<string>();

        protected internal List<string> Install { get; set; } = new List<string>();

        protected internal List<string> BranchesOnly { get; set; } = new List<string>();

        protected internal List<string> Scripts { get; set; } = new List<string>()
        {
            @"export PATH=""$PATH:$HOME/.dotnet/tools""",
            @"dotnet tool install --global FlubuCore.GlobalTool --version 5.1.8"
        };

        protected internal List<string> ScriptsAfter { get; set; } = new List<string>();

        protected internal string Language { get; set; } = "csharp";

        protected internal string Sudo { get; set; } = "required";

        protected internal string DotnetVersion { get; set; } = "3.1.201";

        protected internal string OsImage { get; set; }

        protected internal string Mono { get; set; } = "none";

        protected internal string Dist { get; set; } = "xenial";

        protected internal bool ShouldGenerateOnEachBuild { get; set; }

        protected internal string ConfigFileName { get; set; } = ".travis.generated.yml";

        /// <summary>
        /// Adds 'before_script:' entries.
        /// </summary>
        /// <param name="beforeBuildScript"></param>
        /// <returns></returns>
        public TravisOptions AddBeforeScript(params string[] beforeBuildScript)
        {
            BeforeScript.AddRange(beforeBuildScript);
            return this;
        }

        /// <summary>
        /// Adds 'script:' entries before Flubu target script entry.
        /// </summary>
        /// <param name="scriptBeforeTarget"></param>
        /// <returns></returns>
        public TravisOptions AddScriptsBeforeTarget(params string[] scriptBeforeTarget)
        {
            Scripts.AddRange(scriptBeforeTarget);
            return this;
        }

        /// <summary>
        /// Adds 'script:' entries after Flubu target script entry.
        /// </summary>
        /// <param name="scriptAfterTarget"></param>
        /// <returns></returns>
        public TravisOptions AddScriptsAfterTarget(params string[] scriptAfterTarget)
        {
            ScriptsAfter.AddRange(scriptAfterTarget);
            return this;
        }

        /// <summary>
        /// Adds 'os:' entry. Default linux.
        /// </summary>
        /// <param name="os"></param>
        /// <returns></returns>
        public TravisOptions AddOs(params TravisOs[] os)
        {
            Os.AddRange(os);
            return this;
        }

        /// <summary>
        /// Adds environment variable.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TravisOptions AddEnvironmentVariable(string key, string value)
        {
            EnvironemtVariables.Add($"{key}={value}");
            return this;
        }

        /// <summary>
        /// Adds 'services:' entries.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public TravisOptions AddServices(params string[] services)
        {
            Services.AddRange(services);
            return this;
        }

        /// <summary>
        /// Adds 'installs:' entries.
        /// </summary>
        /// <param name="installs"></param>
        /// <returns></returns>
        public TravisOptions AddInstalls(params string[] installs)
        {
            Install.AddRange(installs);
            return this;
        }

        /// <summary>
        /// Adds 'branches: -only:' entries.
        /// </summary>
        /// <param name="branches"></param>
        /// <returns></returns>
        public TravisOptions AddBranchesOnly(params string[] branches)
        {
            BranchesOnly.AddRange(branches);
            return this;
        }

        /// <summary>
        /// Set's 'dotnet:' version entry
        /// </summary>
        /// <param name="dotnet"></param>
        /// <returns></returns>
        public TravisOptions SetDotnetVersion(string dotnet)
        {
            DotnetVersion = dotnet;
            return this;
        }

        /// <summary>
        /// set's 'mono entry. default is 'none'.
        /// </summary>
        /// <param name="mono"></param>
        /// <returns></returns>
        public TravisOptions SetMono(string mono)
        {
            Mono = mono;
            return this;
        }

        /// <summary>
        /// Set's Osx Image. Default empty
        /// </summary>
        /// <param name="osxImage"></param>
        /// <returns></returns>
        public TravisOptions SetOsXImage(string osxImage)
        {
            OsImage = osxImage;
            return this;
        }

        /// <summary>
        /// Set's 'dist:' entry. Default is 'xenial'.
        /// </summary>
        /// <param name="dist"></param>
        /// <returns></returns>
        public TravisOptions SetDist(string dist)
        {
            Dist = dist;
            return this;
        }

        /// <summary>
        /// Set's config file name. Default '.travis.generated.yml'.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public TravisOptions SetConfigFileName(string fileName)
        {
            ConfigFileName = fileName;
            return this;
        }

        /// <summary>
        /// When set travis configuration file is generated on each build.
        /// </summary>
        /// <returns></returns>
        public TravisOptions GenerateOnEachBuild()
        {
            ShouldGenerateOnEachBuild = true;
            return this;
        }
    }
}
