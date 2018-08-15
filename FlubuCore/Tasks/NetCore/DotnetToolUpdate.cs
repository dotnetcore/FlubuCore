using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetToolUpdate : ExecuteDotnetTaskBase<DotnetToolUpdate>
    {
        private string _description;

        /// <summary>
        /// Installs a tool for use on the command line.
        /// </summary>
        /// <param name="nugetPackageId">NuGet Package Id of the tool to install.</param>
        public DotnetToolUpdate(string nugetPackageId)
            : base(StandardDotnetCommands.Tool)
        {
            WithArguments("update");
            WithArguments(nugetPackageId);
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return "Executes command 'dotnet tool update' with specified arguments..";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        ///  Install user wide as global tool.
        /// </summary>
        /// <returns></returns>
        public DotnetToolUpdate Global()
        {
            WithArguments("-g");
            return this;
        }

        /// <summary>
        /// Location where the tool will be installed.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DotnetToolUpdate ToolInstallationPath(string path)
        {
            WithArguments("--tool-path", path);
            return this;
        }

        /// <summary>
        /// The NuGet configuration file to use.
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        public DotnetToolUpdate NugetConfigFile(string pathToFile)
        {
            WithArguments("--configfile", pathToFile);
            return this;
        }

        /// <summary>
        /// Adds an additional NuGet package source to use during installation.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public DotnetToolUpdate AddNugetSource(string source)
        {
            WithArguments("--add-source", source);
            return this;
        }

        /// <summary>
        /// The target framework to install the tool for.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        public DotnetToolUpdate Framework(string framework)
        {
            WithArguments("--framework", framework);
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        public DotnetToolUpdate Verbosity(VerbosityOptions verbosity)
        {
            WithArguments("--verbosity", verbosity.ToString().ToLower());
            return this;
        }
    }
}
