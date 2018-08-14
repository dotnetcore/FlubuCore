using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetToolInstall : ExecuteDotnetTaskBase<DotnetToolInstall>
    {
        private string _description;

        public DotnetToolInstall(string nugetPackageId)
            : base(StandardDotnetCommands.Tool)
        {
            WithArguments("install");
            WithArguments(nugetPackageId);
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes dotnet command 'tool install'.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Version of the tool package in NuGet.
        /// </summary>
        /// <param name="version">The version</param>
        /// <returns></returns>
        public DotnetToolInstall NugetPackageVersion(string version)
        {
            WithArguments("--version", version);
            return this;
        }

        /// <summary>
        ///  Install user wide as global tool.
        /// </summary>
        /// <returns></returns>
        public DotnetToolInstall Global()
        {
            WithArguments("-g");
            return this;
        }

        public DotnetToolInstall ToolInstallationPath(string path)
        {
            WithArguments("--tool-path", path);
            return this;
        }

        public DotnetToolInstall NugetConfigFile(string pathToFile)
        {
            WithArguments("--configfile", pathToFile);
            return this;
        }

        public DotnetToolInstall AddNugetSource(string source)
        {
            WithArguments("--add-source", source);
            return this;
        }

        public DotnetToolInstall Framework(string framework)
        {
            WithArguments("--framework", framework);
            return this;
        }

        public DotnetToolInstall Verbosity(VerbosityOptions verbosity)
        {
            WithArguments("--verbosity", verbosity.ToString().ToLower());
            return this;
        }
    }
}
