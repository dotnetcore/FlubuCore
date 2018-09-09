using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Tasks.NetCore
{
    public class DotnetToolUninstall : ExecuteDotnetTaskBase<DotnetToolUninstall>
    {
        private string _description;

        /// <summary>
        /// Uninstalls a tool.
        /// </summary>
        /// <param name="nugetPackageId">NuGet Package Id of the tool to uninstall.</param>
        public DotnetToolUninstall(string nugetPackageId)
            : base(StandardDotnetCommands.Tool)
        {
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
            path.MustNotBeNullOrEmpty(ValidationMessages.ParamNotNullOrEmpty, nameof(path));
            WithArguments("--tool-path", path);
            return this;
        }
    }
}
