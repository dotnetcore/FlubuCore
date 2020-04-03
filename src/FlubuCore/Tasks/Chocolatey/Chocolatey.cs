namespace FlubuCore.Tasks.Chocolatey
{
    public class Chocolatey
    {
        /// <summary>
        /// Installs a package or a list of packages (sometimes specified as a packages.config).
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public ChocolateyInstallTask Install(params string[] package)
        {
            return new ChocolateyInstallTask(package);
        }

        /// <summary>
        /// Upgrades a package or a list of packages.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public ChocolateyUpgradeTask Upgrade(params string[] package)
        {
            return new ChocolateyUpgradeTask(package);
        }

        /// <summary>
        /// Uninstalls a package or a list of packages.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public ChocolateyUninstallTask Uninstall(params string[] package)
        {
            return new ChocolateyUninstallTask(package);
        }

        /// <summary>
        /// Chocolatey will generate package specification files for a new package.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ChocolateyNewTask New(string name)
        {
            return new ChocolateyNewTask(name);
        }

        /// <summary>
        /// hocolatey will attempt to package a nuspec into a compiled nupkg.
        /// </summary>
        /// <param name="pathToNuspec"></param>
        public ChocolateyPackTask Pack(string pathToNuspec)
        {
            return new ChocolateyPackTask(pathToNuspec);
        }

        /// <summary>
        /// Chocolatey will attempt to push a compiled nupkg to a package feed.
        /// </summary>
        /// <returns></returns>
        public ChocolateyPushTask Push()
        {
            return new ChocolateyPushTask();
        }
    }
}
