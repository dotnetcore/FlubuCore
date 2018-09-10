using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetToolUninstall : ExecuteDotnetTaskBase<DotnetToolUninstall>
    {
        private readonly string _nugetPackageId;
        private string _description;

        /// <summary>
        /// Uninstalls a tool.
        /// </summary>
        /// <param name="nugetPackageId">NuGet Package Id of the tool to uninstall.</param>
        public DotnetToolUninstall(string nugetPackageId)
            : base(StandardDotnetCommands.Tool)
        {
            _nugetPackageId = nugetPackageId;
            WithArguments("uninstall");
            WithArguments(nugetPackageId);
        }

        protected override string Description
        {
            get => string.IsNullOrEmpty(_description) ? "Executes command 'dotnet tool uninstall' with specified arguments." : _description;

            set => _description = value;
        }

        /// <summary>
        /// Uninstall user wide.
        /// </summary>
        /// <returns></returns>
        public DotnetToolUninstall Global()
        {
            WithArguments("-g");
            return this;
        }

        /// <summary>
        /// Location where the tool was previously installed.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DotnetToolUninstall ToolPath(string path)
        {
            WithArgumentsValueRequired("--tool-path", path);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _nugetPackageId.MustNotBeNullOrEmpty("Nuget package id of the tool to uninstall must not be empty.");
            return base.DoExecute(context);
        }
    }
}
