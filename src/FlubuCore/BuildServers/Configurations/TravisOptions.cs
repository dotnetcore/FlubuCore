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
            @"dotnet tool install --global FlubuCore.GlobalTool --version 5.1.1"
        };

        protected internal string Language { get; set; } = "csharp";

        protected internal string Sudo { get; set; } = "required";

        protected internal string DotnetVersion { get; set; } = "3.1.100";

        protected internal string OsImage { get; set; }

        protected internal string Mono { get; set; } = "none";

        protected internal string Dist { get; set; } = "xenial";

        public TravisOptions AddBeforeBuildScript(params string[] beforeBuildScript)
        {
            BeforeScript.AddRange(beforeBuildScript);
            return this;
        }

        public TravisOptions AddOs(params TravisOs[] os)
        {
            Os.AddRange(os);
            return this;
        }

        public TravisOptions AddEnvironmentVariable(string key, string value)
        {
            EnvironemtVariables.Add($"{key}={value}");
            return this;
        }

        public TravisOptions AddServices(params string[] services)
        {
            Services.AddRange(services);
            return this;
        }

        public TravisOptions AddInstalls(params string[] installs)
        {
            Install.AddRange(installs);
            return this;
        }

        public TravisOptions AddBranchesOnly(params string[] branches)
        {
            BranchesOnly.AddRange(branches);
            return this;
        }

        public TravisOptions SetDotnetVersion(string dotnet)
        {
            DotnetVersion = dotnet;
            return this;
        }

        public TravisOptions SetMono(string mono)
        {
            Mono = mono;
            return this;
        }

        public TravisOptions SetOsXImage(string osxImage)
        {
            OsImage = osxImage;
            return this;
        }

        public TravisOptions SetDist(string dist)
        {
            Dist = dist;
            return this;
        }
    }
}
