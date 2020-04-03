using FlubuCore.Context;
using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.Chocolatey
{
    public class ChocolateyUpgradeTask : ChocolateyBaseTask<ChocolateyUpgradeTask>
    {
        private string[] _packages;

        public ChocolateyUpgradeTask(params string[] package)
        {
            ExecutablePath = "choco";
            WithArguments("upgrade");
            _packages = package;
        }

        protected override string Description { get; set; }

        protected override int DoExecute(ITaskContextInternal context)
        {
            foreach (var package in _packages)
            {
                InsertArgument(1, package);
            }

            base.DoExecute(context);

            return 0;
        }

        /// <summary>
        ///  Source - The source to find the package(s) to install. Special sources include: ruby, webpi, cygwin, windowsfeatures, and python. To specify
        ///  more than one source, pass it with a semi-colon separating the values (-e.g. "'source1;source2'"). Defaults to default feeds.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [ArgKey("--source")]
        public ChocolateyUpgradeTask Source(string source)
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
        public ChocolateyUpgradeTask Version(string version)
        {
            WithArgumentsKeyFromAttribute(version);
            return this;
        }

        /// <summary>
        /// Prerelease - Include Prereleases? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--prerelease")]
        public ChocolateyUpgradeTask IncludePrerelease()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// ForceX86 - Force x86 (32bit) installation on 64 bit systems. Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--forceX86")]
        public ChocolateyUpgradeTask ForceX86()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  InstallArguments - Install Arguments to pass to the native installer in the package. Defaults to unspecified.
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        [ArgKey("--install-arguments")]
        public ChocolateyUpgradeTask InstallArguments(string arguments)
        {
            WithArgumentsKeyFromAttribute(arguments);
            return this;
        }

        [ArgKey("--override-arguments")]
        public ChocolateyUpgradeTask OverrideArguments()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--package-parameters")]
        public ChocolateyUpgradeTask PackageParameters(string parameters)
        {
            WithArgumentsKeyFromAttribute(parameters);
            return this;
        }

        /// <summary>
        /// Apply Install Arguments To Dependencies  - Should install arguments be applied to dependent packages? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--apply-args-to-dependencies")]
        public ChocolateyUpgradeTask ApplyInstallArgumentsToDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Apply Package Parameters To Dependencies  - Should package parameters be applied to dependent packages? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--apply-package-parameters-to-dependencies")]
        public ChocolateyUpgradeTask ApplyPackageParametersToDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// AllowDowngrade - Should an attempt at downgrading be allowed? Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-downgrade")]
        public ChocolateyUpgradeTask AllowDowngrade()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// ForceDependencies - Force dependencies to be reinstalled when force installing package(s). Must be used in conjunction with --force.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force-dependencies")]
        public ChocolateyUpgradeTask ForceDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  IgnoreDependencies - Ignore dependencies when installing package(s).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-dependencies")]
        public ChocolateyUpgradeTask IgnoreDependencies()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Skip Powershell - Do not run chocolateyInstall.ps1. Defaults to false.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-automation-scripts")]
        public ChocolateyUpgradeTask SkipAutomationScripts()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  User - used with authenticated feeds. Defaults to empty.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [ArgKey("--user")]
        public ChocolateyUpgradeTask User(string user)
        {
            WithArgumentsKeyFromAttribute(user);
            return this;
        }

        /// <summary>
        ///  Password - the user's password to the source. Defaults to empty.
        /// </summary>
        /// <param name="paswword"></param>
        /// <returns></returns>
        [ArgKey("--password")]
        public ChocolateyUpgradeTask Password(string paswword)
        {
            WithArgumentsKeyFromAttribute(paswword, maskArg: true);
            return this;
        }

        /// <summary>
        /// Client certificate - PFX pathname for an x509 authenticated feeds. Defaults to empty. Available in 0.9.10+.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [ArgKey("--cert")]
        public ChocolateyUpgradeTask Cert(string path)
        {
            WithArgumentsKeyFromAttribute(path);
            return this;
        }

        /// <summary>
        ///   Certificate Password - the client certificate's password to the source. Defaults to empty. Available in 0.9.10+.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [ArgKey("--certpassword")]
        public ChocolateyUpgradeTask CertPassword(string password)
        {
            WithArgumentsKeyFromAttribute(password, maskArg: true);
            return this;
        }

        /// <summary>
        ///  IgnoreChecksums - Ignore checksums provided by the package. Overrides the default feature 'checksumFiles' set to 'True'. Available in 0.9.9.9+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-checksums")]
        public ChocolateyUpgradeTask IgnoreChecksums()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Except - a comma-separated list of package names that should not be upgraded when upgrading 'all'. Defaults to empty. Available in 0.9.10+.
        /// </summary>
        /// <param name="packages"></param>
        /// <returns></returns>
        [ArgKey("--except")]
        public ChocolateyUpgradeTask Except(string packages)
        {
            WithArgumentsKeyFromAttribute(packages);
            return this;
        }

        /// <summary>
        ///  Skip Packages Not Installed - if a package is not installed, do not install it during the upgrade process. Overrides the default feature
        /// 'skipPackageUpgradesWhenNotInstalled' set to 'False'. Available in 0.1-0.12+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-if-not-installed")]
        public ChocolateyUpgradeTask SkipIfNotInstalled()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--install-if-not-installed")]
        public ChocolateyUpgradeTask InstallIfNotInstalled()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///   Allow Empty Checksums - Allow packages to have empty/missing checksums for downloaded resources from non-secure locations (HTTP, FTP). Use this
        ///   switch is not recommended if using sources that download resources from the internet. Overrides the default feature 'allowEmptyChecksums' set to  'False'. Available in 0.10.0+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-empty-checksums")]
        public ChocolateyUpgradeTask AllowEmptyChecksums()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Allow Empty Checksums Secure - Allow packages to have empty checksums for downloaded resources from secure locations (HTTPS). Overrides the
        /// default feature 'allowEmptyChecksumsSecure' set to 'True'. Available in 0.10.0+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-empty-checksums-secure")]
        public ChocolateyUpgradeTask AllowEmptyChecksumsSecure()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Require Checksums - Requires packages to have checksums for downloaded resources (both non-secure and secure). Overrides the default feature
        /// 'allowEmptyChecksums' set to 'False' and 'allowEmptyChecksumsSecure' set to 'True'. Available in 0.10.0+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--require-checksums")]
        public ChocolateyUpgradeTask RequireCheckSums()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Download Checksum - a user provided checksum for downloaded resources for the package. Overrides the package checksum (if it has one).
        /// Defaults to empty. Available in 0.10.0+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--download-checksum")]
        public ChocolateyUpgradeTask DownloadChecksum(string checksum)
        {
            WithArgumentsKeyFromAttribute(checksum);
            return this;
        }

        /// <summary>
        /// Download Checksum 64bit - a user provided checksum for 64bit downloaded resources for the package. Overrides the package 64-bit checksum (if it has one).
        /// Defaults to same as Download Checksum. Available in 0.10.0+.
        /// </summary>
        /// <param name="checksum"></param>
        /// <returns></returns>
        [ArgKey("--download-checksum-x64")]
        public ChocolateyUpgradeTask DownloadChecksumX64(string checksum)
        {
            WithArgumentsKeyFromAttribute(checksum);
            return this;
        }

        /// <summary>
        /// Download Checksum Type 64bit - a user provided checksum for 64bit downloaded resources for the package. Overrides the package 64-bit
        /// checksum (if it has one). Used in conjunction with Download Checksum  64bit. Available values are 'md5', 'sha1', 'sha256' or 'sha512'.
        /// Defaults to same as Download Checksum Type. Available in 0.10.0+.
        /// </summary>
        /// <param name="checksumType"></param>
        /// <returns></returns>
        [ArgKey("--download-checksum-type")]
        public ChocolateyUpgradeTask DownloadChecksumType(string checksumType)
        {
            WithArgumentsKeyFromAttribute(checksumType);
            return this;
        }

        /// <summary>
        /// IgnorePackageExitCodes - Exit with a 0 for success and 1 for non-succes-s, no matter what package scripts provide for exit codes. Overrides the
        /// default feature 'usePackageExitCodes' set to 'True'. Available in 0.-9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-package-exit-codes")]
        public ChocolateyUpgradeTask IgnorePackageExitCodes()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// UsePackageExitCodes - Package scripts can provide exit codes. Use those  for choco's exit code when non-zero (this value can come from a
        ///    dependency package). Chocolatey defines valid exit codes as 0, 1605, 1614, 1641, 3010.  Overrides the default feature 'usePackageExitCodes' set to 'True'. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--use-package-exit-codes")]
        public ChocolateyUpgradeTask UsePackageExitCodes()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--stop-on-first-package-failure")]
        public ChocolateyUpgradeTask StopOnFirstPackageFailure()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Exit When Reboot Detected - Stop running install, upgrade, or uninstall when a reboot request is detected. Requires 'usePackageExitCodes'
        ///  feature to be turned on. Will exit with either 350 or 1604.  Overrides  the default feature 'exitOnRebootDetected' set to 'False'.  Available in  0.10.12+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--exit-when-reboot-detected")]
        public ChocolateyUpgradeTask ExitWhenRebotDetected()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Ignore Detected Reboot - Ignore any detected reboots if found. Overrides  the default feature 'exitOnRebootDetected' set to 'False'.  Available in 0.10.12+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--ignore-detected-reboot")]
        public ChocolateyUpgradeTask IgnoreDetectedReboot()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Skip Download Cache - Use the original download even if a private CDN cache is available for a package. Overrides the default feature
        /// 'downloadCache' set to 'True'. Available in 0.9.10+. [Licensed editions](https://chocolatey.org/compare) only. See https://chocolatey.org/docs/features-private-cdn.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-download-cache")]
        public ChocolateyUpgradeTask SkipDownloadCache()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        [ArgKey("--use-download-cache")]
        public ChocolateyUpgradeTask UseDownloadCache()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// kip Virus Check - Skip the virus check for downloaded files on this run. Overrides the default feature 'virusCheck' set to 'True'. Available
        /// in 0.9.10+. [Licensed editions](https://chocolatey.org/compare) only. See https://chocolate-y.org/docs/features-virus-check.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--skip-virus-check")]
        public ChocolateyUpgradeTask SkipVirusCheck()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Virus Check - check downloaded files for viruses. Overrides the default feature 'virusCheck' set to 'True'. Available in 0.9.10+. Licensed
        ///    editions only. See https://chocolatey.org/docs/features-virus-check.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--virus-check")]
        public ChocolateyUpgradeTask VirusCheck()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Virus Check Minimum Scan Result Positives - the minimum number of scan result positives required to flag a package. Used when virusScannerType
        ///  is VirusTotal. Overrides the default configuration value 'virusCheckMinimumPositives' set to '5'. Available in 0.9.10+. Licensed editions only. See https://chocolatey.org/docs/features-virus-check.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--viruspositivesmin")]
        public ChocolateyUpgradeTask VirusPositiveMin()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// InstallArgumentsSensitive - Install Arguments to pass to the native installer in the package that are sensitive and you do not want logged.
        ///  Defaults to unspecified. Available in 0.10.1+. [Licensed editions](https://chocolatey.org/compare) only.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        [ArgKey("--install-arguments-sensitive")]
        public ChocolateyUpgradeTask InstallArgumentsSensitive(string argument)
        {
            WithArgumentsKeyFromAttribute(argument, maskArg: true);
           return this;
        }

        /// <summary>
        /// PackageParametersSensitive - Package Parameters to pass the package that
        /// are sensitive and you do not want logged. Defaults to unspecified.
        /// Available in 0.10.1+. [Licensed editions](https://chocolatey.org/compare) only.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [ArgKey("--package-parameters-sensitive")]
        public ChocolateyUpgradeTask PackageParametersSensitive(string parameter)
        {
            WithArgumentsKeyFromAttribute(parameter, separator: "=", maskArg: true);
            WithArguments($"={parameter}", true);
            return this;
        }

        /// <summary>
        /// Maximum Download Rate Bits Per Second - The maximum download rate in  bits per second. '0' or empty means no maximum. A number means that will
        /// be the maximum download rate in bps. Defaults to config setting of '0'.  Available in [licensed editions](https://chocolatey.org/compare) v1.10+ only. See https://chocolate- y.org/docs/features-package-throttle.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ArgKey("--max-download-rate")]
        public ChocolateyUpgradeTask MaxDownloadRate(int value)
        {
            WithArgumentsKeyFromAttribute(value.ToString());
            return this;
        }

        /// <summary>
        /// Reducer Installed Package Size (Package Reducer) - Reduce size of the nupkg file to very small and remove extracted archives and installers.
        /// Overrides the default feature 'reduceInstalledPackageSpaceUsage' set to 'True'. [Licensed editions](https://chocolatey.org/compare) only (version 1.12.0+). See https://chocolate-y.org/docs/features-package-reducer.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--reduce-package-size")]
        public ChocolateyUpgradeTask ReducePackageSize()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Do Not Reduce Installed Package Size - Leave the nupkg and files alone in the package. Overrides the default feature
        /// 'reduceInstalledPackageSpaceUsage' set to 'True'. [Licensed editions](https://chocolatey.org/compare) only (version 1.12.0+). See https://chocolatey.org/docs/features-package-reducer.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-reduce-package-size")]
        public ChocolateyUpgradeTask NoReducePackageSize()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// Reduce Only Nupkg File Size - reduce only the size of nupkg file when using Package Reducer. Overrides the default feature
        /// 'reduceOnlyNupkgSize' set to 'False'. [Licensed editions](https://chocolatey.org/compare) only (version -1.12.0+). See https://chocolatey.org/docs/features-package-reducer.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--reduce-nupkg-only")]
        public ChocolateyUpgradeTask ReduceNupkgOnly()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}
