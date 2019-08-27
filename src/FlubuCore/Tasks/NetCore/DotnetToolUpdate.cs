using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetToolUpdate : ExecuteDotnetTaskBase<DotnetToolUpdate>
    {
        private readonly string _nugetPackageId;
        private string _description;

        /// <summary>
        /// Installs a tool for use on the command line.
        /// </summary>
        /// <param name="nugetPackageId">NuGet Package Id of the tool to install.</param>
        public DotnetToolUpdate(string nugetPackageId)
            : base(StandardDotnetCommands.Tool)
        {
            _nugetPackageId = nugetPackageId;
            WithArguments("update");
            WithArguments(nugetPackageId);
        }

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Executes command 'dotnet tool update' with specified arguments." : _description;

            set => _description = value;
        }

        /// <summary>
        /// Update user wide as global tool.
        /// </summary>
        /// <returns></returns>
        [ArgKey("-g")]
        public DotnetToolUpdate Global()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Location where the tool will be installed.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ArgKey("--tool-path")]
        public DotnetToolUpdate ToolInstallationPath(string path)
        {
            WithArgumentsKeyFromAttribute(path);
            return this;
        }

        /// <summary>
        /// The NuGet configuration file to use.
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <returns></returns>
        [ArgKey("--configfile")]
        public DotnetToolUpdate NugetConfigFile(string pathToFile)
        {
            WithArgumentsKeyFromAttribute(pathToFile);
            return this;
        }

        /// <summary>
        /// Adds an additional NuGet package source to use during installation.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public DotnetToolUpdate AddNugetSource(string source)
        {
            WithArgumentsKeyFromAttribute(source);
            return this;
        }

        /// <summary>
        /// The target framework to install the tool for.
        /// </summary>
        /// <param name="framework"></param>
        /// <returns></returns>
        [ArgKey("--framework")]
        public DotnetToolUpdate Framework(string framework)
        {
            WithArgumentsKeyFromAttribute(framework);
            return this;
        }

        /// <summary>
        /// Set the verbosity level of the command.
        /// </summary>
        /// <param name="verbosity"></param>
        /// <returns></returns>
        [ArgKey("--verbosity")]
        public DotnetToolUpdate Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString().ToLower());
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _nugetPackageId.MustNotBeNullOrEmpty("Nuget package id of the tool to update must not be empty.");
            return base.DoExecute(context);
        }
    }
}
