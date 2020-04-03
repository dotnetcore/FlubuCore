using FlubuCore.Tasks.Attributes;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Chocolatey
{
    public class ChocolateyBaseTask<TTask> : ExternalProcessTaskBase<int, TTask>
        where TTask : class, ITask
    {
        protected override string Description { get; set; }

        protected override string KeyValueSeparator { get; } = "=";

        /// <summary>
        /// Debug - Show debug messaging.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--debug")]
        public TTask Debug()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  Verbose - Show verbose messaging. Very verbose messaging, avoid using under normal circumstances.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--verbose")]
        public TTask Verbose()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        /// Show trace messaging. Very, very verbose trace messaging. Avoid except when needing super low-level .NET Framework debugging.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--trace")]
        public TTask Trace()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  No Color - Do not show colorization in logging output. This overrides the feature 'logWithoutColor', set to 'False'.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--no-color")]
        public TTask NoColor()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  AcceptLicense - Accept license dialogs automatically. Reserved for future use.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--accept-licence")]
        public TTask AcceptLicence()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        /// Confirm all prompts - Chooses affirmative answer instead of prompting.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--confirm")]
        public TTask Confirm()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        /// Force - force the behavior. Do not use force during normal operation -it subverts some of the smart behavior for commands.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--force")]
        public TTask Force()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  NoOp / WhatIf - Don't actually do anything.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--noop")]
        public TTask NoOp()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  LimitOutput - Limit the output to essential information.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--limit-output")]
        public TTask LimitOutput()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  CommandExecutionTimeout (in seconds) - The time to allow a command to finish before timing out. Overrides the default execution timeout in the configuration of 2700 seconds.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [ArgKey("--timeout")]
        public TTask Timeout(int timeout)
        {
            WithArgumentsKeyFromAttribute(timeout.ToString());
            return this as TTask;
        }

        /// <summary>
        ///  CacheLocation - Location for download cache, defaults to %TEMP% or value in chocolatey.config file.
        /// </summary>
        /// <param name="cacheLocation"></param>
        /// <returns></returns>
        [ArgKey("--cache-location")]
        public TTask CacheLocation(string cacheLocation)
        {
            WithArgumentsKeyFromAttribute(cacheLocation);
            return this as TTask;
        }

        /// <summary>
        /// AllowUnofficialBuild - When not using the official build you must set this flag for choco to continue.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--allow-unofficial")]
        public TTask AllowUnofficial()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  FailOnStandardError - Fail on standard error output (stderr), typically received when running external commands during install providers. This
        /// overrides the feature failOnStandardError.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--fail-on-stderr")]
        public TTask FailOnStandardError()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        /// Proxy Location - Explicit proxy location. Overrides the default proxy location of ''.
        /// </summary>
        /// <param name="proxy"></param>
        /// <returns></returns>
        [ArgKey("--proxy")]
        public TTask Proxy(string proxy)
        {
            WithArgumentsKeyFromAttribute(proxy);
            return this as TTask;
        }

        /// <summary>
        /// Proxy User Name - Explicit proxy user (optional). Requires explicitly proxy (`--proxy` or config setting). Overrides the default proxy user of '123'.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [ArgKey("--proxy-user")]
        public TTask ProxyUser(string user)
        {
            WithArgumentsKeyFromAttribute(user);
            return this as TTask;
        }

        /// <summary>
        /// proxy Password - Explicit proxy password (optional) to be used with username. Requires explicity proxy (`--proxy` or config setting) and user name.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [ArgKey("--proxy-password")]
        public TTask ProxyPassword(string password)
        {
            WithArgumentsKeyFromAttribute(password, true);
            return this as TTask;
        }

        /// <summary>
        /// ProxyBypassList - Comma separated list of regex locations to bypass on proxy. Requires explicity proxy (`--proxy` or config setting). Overrides the default proxy bypass list of ''.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [ArgKey("--proxy-bypass-list")]
        public TTask ProxyBypassList(string list)
        {
            WithArgumentsKeyFromAttribute(list);
            return this as TTask;
        }

        /// <summary>
        ///    Proxy Bypass On Local - Bypass proxy for local connections. Requires explicity proxy (`--proxy` or config setting). Overrides the default proxy bypass on local setting of 'True'.
        /// </summary>
        /// <returns></returns>
        [ArgKey("--proxy-bypass-on-local")]
        public TTask ProxyBypassOnLocal()
        {
            WithArgumentsKeyFromAttribute();
            return this as TTask;
        }

        /// <summary>
        ///  Log File to output to in addition to regular loggers.
        /// </summary>
        /// <param name="logFile"></param>
        /// <returns></returns>
        [ArgKey("--log-file")]
        public TTask LogFile(string logFile)
        {
            WithArgumentsKeyFromAttribute(logFile);
            return this as TTask;
        }
    }
}
