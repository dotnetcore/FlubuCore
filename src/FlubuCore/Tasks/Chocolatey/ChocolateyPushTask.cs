using FlubuCore.Tasks.Attributes;

namespace FlubuCore.Tasks.Chocolatey
{
    public class ChocolateyPushTask : ChocolateyBaseTask<ChocolateyPushTask>
    {
        private string _path;

        public ChocolateyPushTask()
        {
            ExecutablePath = "choco";
            WithArguments("push");
        }

        public ChocolateyPushTask PathToNupkg(string path)
        {
            _path = path;
            return this;
        }

        /// <summary>
        ///   Source - The source we are pushing the package to. Use https://push.chocolatey.org/ to push to [community feed](https://chocolatey.org/packages).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [ArgKey("--source")]
        public ChocolateyPushTask Source(string source)
        {
            WithArgumentsKeyFromAttribute(source);
            return this;
        }

        /// <summary>
        ///  ApiKey - The api key for the source. If not specified (and not local file source), does a lookup. If not specified and one is not found for an https source, push will fail.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [ArgKey("--key")]
        public ChocolateyPushTask ApiKey(string key)
        {
            WithArgumentsKeyFromAttribute(key, true);
            return this;
        }

        /// <summary>
        /// Timeout (in seconds) - The time to allow a package push to occur before timing out. Defaults to execution timeout 2700.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [ArgKey("-t")]
        public ChocolateyPushTask Timeout(int timeout)
        {
            WithArgumentsKeyFromAttribute(timeout.ToString());
            return this;
        }
    }
}
