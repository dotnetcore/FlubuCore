using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetToolInstall : ExecuteDotnetTaskBase<DotnetToolInstall>
    {
        private readonly string _nugetPackageId;

        private string _description;

        /// <summary>
        /// Installs a tool for use on the command line.
        /// </summary>
        /// <param name="nugetPackageId">NuGet Package Id of the tool to install.</param>
        public DotnetToolInstall(string nugetPackageId)
            : base(StandardDotnetCommands.Tool)
        {
            _nugetPackageId = nugetPackageId;
            WithArguments("install");
            WithArguments(nugetPackageId);
        }

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Executes command 'dotnet tool install' with specified arguments." : _description;

            set => _description = value;
        }

        /// <summary>
        /// Version of the tool package in NuGet.
        /// </summary>
        /// <param name="version">The version</param>
        /// <returns></returns>
        [ArgKey("--version")]
        public DotnetToolInstall NugetPackageVersion(string version)
        {
            WithArgumentsKeyFromAttribute(version);
            return this;
        }

        /// <summary>
        ///  Install user wide as global tool.
        /// </summary>
        /// <returns></returns>
        [ArgKey("-g")]
        public DotnetToolInstall Global()
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
        public DotnetToolInstall ToolInstallationPath(string path)
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
        public DotnetToolInstall NugetConfigFile(string pathToFile)
        {
            WithArgumentsKeyFromAttribute(pathToFile);
            return this;
        }

        /// <summary>
        /// Adds an additional NuGet package source to use during installation.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [ArgKey("--add-source")]
        public DotnetToolInstall AddNugetSource(string source)
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
        public DotnetToolInstall Framework(string framework)
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
        public DotnetToolInstall Verbosity(VerbosityOptions verbosity)
        {
            WithArgumentsKeyFromAttribute(verbosity.ToString());
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _nugetPackageId.MustNotBeNullOrEmpty("Nuget package id of the tool to install must not be empty.");
            return base.DoExecute(context);
        }
    }
}
