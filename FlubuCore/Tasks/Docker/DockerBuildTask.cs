using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Docker
{
    public class DockerBuildTask : ExternalProcessTaskBase<DockerBuildTask>
    {
        private readonly string _pathOrUrl;
        private string _description;

        public DockerBuildTask(string pathOrUrl)
        {
            WithArguments("build");
            _pathOrUrl = pathOrUrl;
        }

        protected override string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    return $"Executes docker command 'build' with specified option.";
                }

                return _description;
            }

            set { _description = value; }
        }

        /// <summary>
        /// Image name and optionally a tag in the 'name:tag' format.
        /// </summary>
        /// <param name="nameAndTag"></param>
        /// <returns></returns>
        public DockerBuildTask ImageNameAndTag(string nameAndTag)
        {
            WithArgumentsValueRequired("--tag", nameAndTag);
            return this;
        }

        public DockerBuildTask UseDockerFile(string pathToDockerFile)
        {
            WithArgumentsValueRequired("--file", pathToDockerFile);
            return this;
        }

        /// <summary>
        /// --add-host
        /// Add a custom host-to-IP mapping (host:ip)
        /// </summary>
        public DockerBuildTask AddHost(string host)
        {
            WithArgumentsValueRequired("--add-host", host);
            return this;
        }

        /// <summary>
        /// --build-arg
        /// Set build-time variables
        /// </summary>
        public DockerBuildTask BuildArg(string buildArg)
        {
            WithArgumentsValueRequired("--build-arg", buildArg);
            return this;
        }

        /// <summary>
        /// --cache-from
        /// Images to consider as cache sources
        /// </summary>
        public DockerBuildTask CacheFromImages(string image)
        {
            WithArgumentsValueRequired("--cache-from", image);
            return this;
        }

        /// <summary>
        /// --cgroup-parent
        /// Optional parent cgroup for the container
        /// </summary>
        public DockerBuildTask CgroupParent(string parent)
        {
            WithArgumentsValueRequired("--cgroup-parent", parent);
            return this;
        }

        /// <summary>
        /// --compress
        /// Compress the build context using gzip
        /// </summary>
        public DockerBuildTask Compress()
        {
            WithArguments("--compress");
            return this;
        }

        /// <summary>
        /// --cpu-period
        /// default: 0
        /// Limit the CPU CFS (Completely Fair Scheduler) period
        /// </summary>
        public DockerBuildTask CpuPeriod(int period)
        {
            WithArgumentsValueRequired("--cpu-period", period.ToString());
            return this;
        }

        /// <summary>
        /// --cpu-quota
        /// default: 0
        /// Limit the CPU CFS (Completely Fair Scheduler) quota
        /// </summary>
        public DockerBuildTask CpuQuota(int quota)
        {
            WithArgumentsValueRequired("--cpu-quota", quota.ToString());
            return this;
        }

        /// <summary>
        /// --cpuset-cpus
        /// CPUs in which to allow execution (0-3, 0,1)
        /// </summary>
        public DockerBuildTask CpusetCpus(string cpus)
        {
            WithArgumentsValueRequired("--cpuset-cpus", cpus);
            return this;
        }

        /// <summary>
        /// --cpuset-mems
        /// MEMs in which to allow execution (0-3, 0,1)
        /// </summary>
        public DockerBuildTask CpusetMems(string mems)
        {
            WithArgumentsValueRequired("--cpuset-mems", mems);
            return this;
        }

        /// <summary>
        /// --cpu-shares, -c
        /// default: 0
        /// CPU shares (relative weight)
        /// </summary>
        public DockerBuildTask CpuShares(int shares)
        {
            WithArgumentsValueRequired("--cpu-shares", shares.ToString());
            return this;
        }

        /// <summary>
        /// --disable-content-trust
        /// default: true
        /// Skip image verification
        /// </summary>
        public DockerBuildTask DisableContentTrust()
        {
            WithArguments("--disable-content-trust");
            return this;
        }

        /// <summary>
        /// --force-rm
        /// default: false
        /// Always remove intermediate containers
        /// </summary>
        public DockerBuildTask ForceRm()
        {
            WithArguments("--force-rm");
            return this;
        }

        /// <summary>
        /// --iidfile
        /// Write the image ID to the file
        /// </summary>
        public DockerBuildTask IidFile(string file)
        {
            WithArgumentsValueRequired("--iidfile", file);
            return this;
        }

        /// <summary>
        /// --isolation
        /// Container isolation technology
        /// </summary>
        public DockerBuildTask Isolation(string isolation)
        {
            WithArgumentsValueRequired("--isolation", isolation);
            return this;
        }

        /// <summary>
        /// --label
        /// Set metadata for an image
        /// </summary>
        public DockerBuildTask Label(string label)
        {
            WithArgumentsValueRequired("--label", label);
            return this;
        }

        /// <summary>
        /// --memory, -m
        /// Memory limit
        /// </summary>
        public DockerBuildTask MemoryLimit(string limit)
        {
            WithArgumentsValueRequired("--memory", limit);
            return this;
        }

        /// <summary>
        /// --memory-swap
        /// Swap limit equal to memory plus swap: &#39;-1&#39; to enable unlimited swap
        /// </summary>
        public DockerBuildTask MemorySwap(string value)
        {
            WithArgumentsValueRequired("--memory-swap", value);
            return this;
        }

        /// <summary>
        /// --network
        /// default: default
        /// Set the networking mode for the RUN instructions during build
        /// </summary>
        public DockerBuildTask Network(string network)
        {
            WithArgumentsValueRequired("--network", network);
            return this;
        }

        /// <summary>
        /// --no-cache
        /// Do not use cache when building the image
        /// </summary>
        public DockerBuildTask NoCache()
        {
            WithArguments("--no-cache");
            return this;
        }

        /// <summary>
        /// --platform
        /// Set platform if server is multi-platform capable
        /// </summary>
        public DockerBuildTask Platform(string platform)
        {
            WithArgumentsValueRequired("--platform", platform);
            return this;
        }

        /// <summary>
        /// --pull
        /// default: false
        /// Always attempt to pull a newer version of the image
        /// </summary>
        public DockerBuildTask PullNewerVersionOfTheImage()
        {
            WithArguments("--pull");
            return this;
        }

        /// <summary>
        /// --quiet, -q
        /// Suppress the build output and print image ID on success
        /// </summary>
        public DockerBuildTask Quiet()
        {
            WithArguments("--quit");
            return this;
        }

        /// <summary>
        /// --rm
        /// default: true
        /// Remove intermediate containers after a successful build
        /// </summary>
        public DockerBuildTask RemoveIntermediateContainer()
        {
            WithArguments("--rm");
            return this;
        }

        /// <summary>
        /// --security-opt
        /// Security options
        /// </summary>
        public DockerBuildTask SecurityOptions(string option)
        {
            WithArgumentsValueRequired("--security-opt", option);
            return this;
        }

        /// <summary>
        /// --shm-size
        /// Size of /dev/shm
        /// </summary>
        public DockerBuildTask ShmSize(string size)
        {
            WithArgumentsValueRequired("--shm-size", size);
            return this;
        }

        /// <summary>
        /// --squash
        /// Squash newly built layers into a single new layer
        /// </summary>
        public DockerBuildTask Squash()
        {
            WithArguments("--squash");
            return this;
        }

        /// <summary>
        /// --stream
        /// Stream attaches to server to negotiate build context
        /// </summary>
        public DockerBuildTask Stream()
        {
            WithArguments("--stream");
            return this;
        }

        /// <summary>
        /// --target
        /// Set the target build stage to build.
        /// </summary>
        public DockerBuildTask Target(string target)
        {
            WithArgumentsValueRequired("--target", target);
            return this;
        }

        /// <summary>
        /// --ulimit
        /// Ulimit options
        /// </summary>
        public DockerBuildTask Ulimit(string option)
        {
            WithArgumentsValueRequired("--ulimit", option);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _pathOrUrl.MustNotBeNullOrEmpty("PATH | URL must not be empty. run 'docker build --help' or see docker documentation online for help.");
            WithArguments(_pathOrUrl);
            return base.DoExecute(context);
        }
    }
}
