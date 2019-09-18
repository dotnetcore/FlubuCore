using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Chocolatey;

namespace FlubuCore.Infrastructure.Terminal.Commands
{
    public static class ChocolateyCommands
    {
        public static Dictionary<string, Type> SupportedExternalProcesses { get; } = new Dictionary<string, Type>()
        {
            { "choco install", typeof(ChocolateyInstallTask) },
            { "choco new", typeof(ChocolateyNewTask) },
            { "choco pack", typeof(ChocolateyPackTask) },
            { "choco push", typeof(ChocolateyPushTask) },
            { "choco uninstall", typeof(ChocolateyUninstallTask) },
            { "choco upgrade", typeof(ChocolateyUpgradeTask) },
        };

        public static KeyValuePair<string, IReadOnlyCollection<Hint>> ChocoCommandHints { get; } = new KeyValuePair<string, IReadOnlyCollection<Hint>>("choco", new List<Hint>()
        {
            new Hint() { Name = "install", Help = "Installs a package or a list of packages (sometimes specified as a packages.config)." },
            new Hint() { Name = "new", Help = "Chocolatey will generate package specification files for a new package." },
            new Hint() { Name = "pack", Help = "Chocolatey will attempt to package a nuspec into a compiled nupkg." },
            new Hint() { Name = "push", Help = "Chocolatey will attempt to push a compiled nupkg to a package feed." },
            new Hint() { Name = "uninstall", Help = "Uninstalls a package or a list of packages." },
            new Hint() { Name = "upgrade", Help = "Upgrades a package or a list of packages." },
        });
    }
}
