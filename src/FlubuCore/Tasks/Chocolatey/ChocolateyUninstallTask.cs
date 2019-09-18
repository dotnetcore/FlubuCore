using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.Chocolatey
{
    public class ChocolateyUninstallTask : ChocolateyBaseTask<ChocolateyUninstallTask>
    {
        private string[] _packages;

        public ChocolateyUninstallTask(params string[] package)
        {
            ExecutablePath = "choco";
            WithArguments("uninstall");
            _packages = package;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            foreach (var package in _packages)
            {
                InsertArgument(1, package);
            }

            return base.DoExecute(context);
        }

        /// <summary>
        ///  Source - The source to find the package(s) to install. Special sources include: ruby, webpi, cygwin, windowsfeatures, and python. To specify
        ///  more than one source, pass it with a semi-colon separating the values (-e.g. "'source1;source2'"). Defaults to default feeds.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [ArgKey("--source")]
        public ChocolateyUninstallTask Source(string source)
        {
            WithArgumentsKeyFromAttribute(source);
            return this;
        }

        /// <summary>
        /// Version - A specific version to install. Defaults to unspecified.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("--version")]
        public ChocolateyUninstallTask Version(string version)
        {
            WithArgumentsKeyFromAttribute(version);
            return this;
        }

        /// <summary>
        ///  InstallArguments - Install Arguments to pass to the native installer in the package. Defaults to unspecified.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        [ArgKey("--uninstall-arguments")]
        public ChocolateyUninstallTask UninstallArguments(string argument)
        {
            WithArgumentsKeyFromAttribute(argument);
            return this;
        }

        [ArgKey("--override-arguments")]
        public ChocolateyUninstallTask OverrideArguments()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--package-parameters")]
        public ChocolateyUninstallTask PackageParameters(string parameters)
        {
            WithArgumentsKeyFromAttribute(parameters);
            return this;
        }

        /// <summary>
        /// Apply Install Arguments To Dependencies  - Should install arguments be applied to dependent packages? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--apply-args-to-dependencies")]
        public ChocolateyUninstallTask ApplyInstallArgumentsToDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Apply Package Parameters To Dependencies  - Should package parameters be applied to dependent packages? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--apply-package-parameters-to-dependencies")]
        public ChocolateyUninstallTask ApplyPackageParametersToDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  AllowMultipleVersions - Should multiple versions of a package be installed? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-multiple-versions")]
        public ChocolateyUninstallTask AllowMultipleVersions()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// RemoveDependencies - Uninstall dependencies when uninstalling package(s). Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--remove-dependencies")]
        public ChocolateyUninstallTask RemoveDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Skip Powershell - Do not run chocolateyUninstall.ps1. Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-automation-scripts")]
        public ChocolateyUninstallTask SkipAutomationScripts()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  IgnorePackageExitCodes - Exit with a 0 for success and 1 for non-succes-s, no matter what package scripts provide for exit codes. Overrides the
        ///  default feature 'usePackageExitCodes' set to 'True'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-package-exit-codes")]
        public ChocolateyUninstallTask IgnorePackageExitCodes()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// UsePackageExitCodes - Package scripts can provide exit codes. Use those for choco's exit code when non-zero (this value can come from a dependency package). Chocolatey defines valid exit codes as 0, 1605,
        /// 1614, 1641, 3010. Overrides the default feature 'usePackageExitCodes' set to 'True'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--use-package-exit-codes")]
        public ChocolateyUninstallTask UsePackageExitCodes()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// UseAutoUninstaller - Use auto uninstaller service when uninstalling. Overrides the default feature 'autoUninstaller' set to 'True'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--use-autouninstaller")]
        public ChocolateyUninstallTask UseAutoUninstaller()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  SkipAutoUninstaller - Skip auto uninstaller service when uninstalling. Overrides the default feature 'autoUninstaller' set to 'True'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-autouninstaller")]
        public ChocolateyUninstallTask SkipAutoInstaller()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// FailOnAutoUninstaller - Fail the package uninstall if the auto uninstaller reports and error. Overrides the default feature 'failOnAutoUninstaller' set to 'False'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--fail-on-autouninstaller")]
        public ChocolateyUninstallTask FailOnAutoUninstaller()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  --ignoreautouninstallerfailure, --ignore-autouninstaller-failure Ignore Auto Uninstaller Failure - Do not fail the package if auto uninstaller reports an error. Overrides the default feature 'failOnAutoUninstaller' set to 'False'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-autouninstaller-failure")]
        public ChocolateyUninstallTask IgnoreAutoUninstallerFailure()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Stop On First Package Failure - stop running install, upgrade or uninstall on first package failure instead of continuing with others.
        ///  Overrides the default feature 'stopOnFirstPackageFailure' set to 'False'. Available in 0.10.4+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--stop-on-first-package-failure")]
        public ChocolateyUninstallTask StopOnFirstPackageFailure()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Exit When Reboot Detected - Stop running install, upgrade, or uninstall when a reboot request is detected. Requires 'usePackageExitCodes'
        /// feature to be turned on. Will exit with either 350 or 1604.  Overrides the default feature 'exitOnRebootDetected' set to 'False'.  Available in 0.10.12+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--exit-when-reboot-detected")]
        public ChocolateyUninstallTask ExitWhenRebootDetected()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Ignore Detected Reboot - Ignore any detected reboots if found. Overrides the default feature 'exitOnRebootDetected' set to 'False'.  Available in 0.10.12+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-detected-reboot")]
        public ChocolateyUninstallTask IgnoreDetectedReboot()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  From Programs and Features - Uninstalls a program from programs and features. Name used for id must be a match or a wildcard (*) to Display
        /// Name in Programs and Features. Available in [licensed editions](https://chocolatey.org/compare) only (licensed version 1.8.0+) and requires v0.10.4+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--from-programs-and-features")]
        public ChocolateyUninstallTask FromProgramsAndFeatures()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}
