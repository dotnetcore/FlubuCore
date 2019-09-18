using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.Chocolatey
{
    public class ChocolateyPackTask : ChocolateyBaseTask<ChocolateyPackTask>
    {
        public ChocolateyPackTask(string pathToNuspec)
        {
            ExecutablePath = "choco";
            WithArguments("pack");
            WithArguments(pathToNuspec);
        }

        [ArgKey("--version")]
        public ChocolateyPackTask Version(string version)
        {
            WithArgumentsKeyFromAttribute(version);
            return this;
        }

        [ArgKey("--outdir")]
        public ChocolateyPackTask OutputDirectory(string directory)
        {
            WithArgumentsKeyFromAttribute(directory);
            return this;
        }
    }
}
