using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.Chocolatey
{
    public class ChocolateyNewTask : ChocolateyBaseTask<ChocolateyNewTask>
    {
        public ChocolateyNewTask(string name)
        {
            ExecutablePath = "choco";
            WithArguments("new", name);
        }

        [ArgKey("--automaticpackage")]
        public ChocolateyNewTask AutomaticPackage()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// TemplateName - Use a named template in C:\ProgramData\chocolatey\templates\templatename instead of built-in
        ///  template. Available in 0.9.9.9+. Manage templates as packages in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--template-name")]
        public ChocolateyNewTask Template(string name)
        {
            WithArgumentsKeyFromAttribute(name);
            return this;
        }

        /// <summary>
        ///  Version - the version of the package. Can also be passed as the property;.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [ArgKey("--version")]
        public ChocolateyNewTask Version(string version)
        {
            WithArgumentsKeyFromAttribute(version);
            return this;
        }

        /// <summary>
        /// Maintainer - the name of the maintainer. Can also be passed as the property MaintainerName=somevalue.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ArgKey("--maintainer")]
        public ChocolateyNewTask Maintainer(string name)
        {
            WithArgumentsKeyFromAttribute(name);
            return this;
        }

        /// <summary>
        /// OutputDirectory - Specifies the directory for the created Chocolatey package file. If not specified, uses the current directory. Available in.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        [ArgKey("--outdir")]
        public ChocolateyNewTask OutputDirectory(string directory)
        {
            WithArgumentsKeyFromAttribute(directory);
            return this;
        }

        /// <summary>
        ///  BuiltInTemplate - Use the original built-in template instead of any override. Available in 0.9.10+.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--use-built-in-template")]
        public ChocolateyNewTask UseBuiltInTemplate()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        /// detection (native installer, zip, patch/upgrade file, or remote url to download first) to completely create a package with proper silent
        /// arguments! Can be 32-bit or 64-bit architecture.  Available in licensed editions only (licensed version 1.4.0+, url/zip starting in 1.6.0). See https://chocolatey.org/docs/features-create-packages-from-installers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ArgKey("--url")]
        public ChocolateyNewTask FileOrUrl(string value)
        {
            WithArgumentsKeyFromAttribute(value);
            return this;
        }

        /// <summary>
        ///  Use Original Files Location - when using file or url, use the original location in packaging. Available in [licensed editions](https://chocolatey.org/compare) only (licensed version 1.6.0+).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--use-original-files-location")]
        public ChocolateyNewTask UseOriginalFilesLocation()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///   Download Checksum - checksum to verify File/Url with. Defaults to empty. Available in [licensed editions](https://chocolatey.org/compare) only (licensed version 1.7.0+).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ArgKey("--download-checksum")]
        public ChocolateyNewTask DownloadCheckSum(string value)
        {
            WithArgumentsKeyFromAttribute(value);
            return this;
        }

        /// <summary>
        /// Download Checksum Type - checksum type for File/Url (and optional separate 64-bit files when specifying both). Used in conjunction with
        /// Download Checksum and Download Checksum 64-bit. Available values are 'md5', 'sha1', 'sha256' or 'sha512'. Defaults to 'sha256'.  Available in Business editions only (licensed version 1.7.0+).
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [ArgKey("--download-checksum-type")]
        public ChocolateyNewTask DownloadChecksumType(string value)
        {
            WithArgumentsKeyFromAttribute(value);
            return this;
        }

        /// <summary>
        /// Pause on Error - Pause when there is an error with creating the package. Available in [licensed editions](https://chocolatey.org/compare) only (licensed version 1.7.0+).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--pause-on-error")]
        public ChocolateyNewTask PauseOnError()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Build Package - Attempt to compile the package after creating it.  Available in [licensed editions](https://chocolatey.org/compare) only (licensed version 1.7.0+).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--build-packages")]
        public ChocolateyNewTask BuildPackages()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>        /// Generate Packages From Installed Software - Generate packages from the installed software on a system (does not handle dependencies). Available in Business editions only (licensed version 1.8.0+).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--from-programs-and-features")]
        public ChocolateyNewTask FromProgramsAndFeatures()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }

        /// <summary>
        ///  Include Architecture in Package Name - Leave x86, x64, 64-bit, etc as part of the package id. Default setting is to remove architecture. Available in Business editions only (licensed version 1.8.0+).
        /// </summary>
        /// <returns></returns>
        [ArgKey("--include-architecture-in-name")]
        public ChocolateyNewTask IncludeArchitectureInName()
        {
            WithArgumentsKeyFromAttribute();
            return this;
        }
    }
}
