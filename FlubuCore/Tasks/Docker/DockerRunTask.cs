using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Context;
using FlubuCore.Tasks.Process;

namespace FlubuCore.Tasks.Docker
{
    public class DockerRunTask : ExternalProcessTaskBase<DockerBuildTask>
    {
        private readonly string _image;
        private readonly string _command;
        private readonly string[] _imageArgs;

        public DockerRunTask(string image, string command, params string[] imageArgs)
        {
            _image = image;
            _command = command;
            _imageArgs = imageArgs;
        }

        protected override string Description { get; set; }

        /// <summary>
        /// --add-host
        /// Add a custom host-to-IP mapping (host:ip)
        /// </summary>
        public DockerRunTask AddHost(string mapping)
        {
            WithArgumentsValueRequired("--add-host", mapping);
            return this;
        }

        /// <summary>
        /// --attach, -a
        /// Attach to STDIN, STDOUT or STDERR.
        /// </summary>
        public DockerRunTask Attach(string attach)
        {
            WithArgumentsValueRequired("--attach", attach);
            return this;
        }

        /// <summary>
        /// --blkio-weight-device
        /// Block IO weight (relative device weight).
        /// </summary>
        public DockerRunTask BlkioWeightDevice(uint blkioWeightDevice)
        {
            WithArgumentsValueRequired("--blkio-weight-device", blkioWeightDevice.ToString());
            return this;
        }

        /// <summary>
        /// --cap-add
        /// Add Linux capabilities.
        /// </summary>
        public DockerRunTask CapAdd(string capAdd)
        {
            WithArgumentsValueRequired("--cap-add", capAdd);
            return this;
        }

        /// <summary>
        /// --cap-drop
        /// Drop Linux capabilities.
        /// </summary>
        public DockerRunTask CapDrop(string capDrop)
        {
            WithArgumentsValueRequired("--cap-drop", capDrop);
            return this;
        }

        /// <summary>
        /// --cgroup-parent
        /// Optional parent cgroup for the container.
        /// </summary>
        public DockerRunTask CgroupParant(string cgroupParent)
        {
            WithArgumentsValueRequired("--cgroup-parent", cgroupParent);
            return this;
        }

        /// <summary>
        /// --cidfile
        /// Write the container ID to the file.
        /// </summary>
        public DockerRunTask CIdFile(string file)
        {
            WithArgumentsValueRequired("--cifdile", file);
            return this;
        }

        /// <summary>
        /// --cpu-count
        /// CPU count (Windows only). default: 0.
        /// </summary>
        public DockerRunTask CpuCount(uint cpucount)
        {
            WithArgumentsValueRequired("--cpu-count", cpucount.ToString());
            return this;
        }

        /// <summary>
        /// --cpu-percent
        /// CPU percent (Windows only) default: 0.
        /// </summary>
        public DockerRunTask CpuPercent(uint cpuPercent)
        {
            WithArgumentsValueRequired("--cpu-percent", cpuPercent.ToString());
            return this;
        }

        /// <summary>
        /// --cpu-period
        /// Limit CPU CFS (Completely Fair Scheduler) period. default: 0
        /// </summary>
        public DockerRunTask CpuPeriod(uint cpuPeriod)
        {
            WithArgumentsValueRequired("--cpu-period", cpuPeriod.ToString());
            return this;
        }

        /// <summary>
        /// --cpu-quota
        /// default: 0
        /// Limit CPU CFS (Completely Fair Scheduler) quota
        /// </summary>
        public DockerRunTask CpuQuota(uint cpuQuota)
        {
            WithArgumentsValueRequired("--cpu-quota", cpuQuota.ToString());
            return this;
        }

        /// <summary>
        /// --cpu-rt-period
        /// Limit CPU real-time period in microseconds. default: 0.
        /// </summary>
        public DockerRunTask CpuRealTimePeriod(uint cpuRtPeriod)
        {
            WithArgumentsValueRequired("--cpu-rt-period", cpuRtPeriod.ToString());
            return this;
        }

        /// <summary>
        /// --cpu-rt-runtime
        /// default: 0
        /// Limit CPU real-time runtime in microseconds
        /// </summary>
        public DockerRunTask CpuRealTimeRuntime(ulong cpuRtRuntime)
        {
            WithArgumentsValueRequired("--cpu-rt-runtime", cpuRtRuntime.ToString());
            return this;
        }

        /// <summary>
        /// --cpus
        /// Number of CPUs
        /// </summary>
        public DockerRunTask Cpus(uint cpus)
        {
            WithArgumentsValueRequired("--cpus", cpus.ToString());
            return this;
        }

        /// <summary>
        /// --cpuset-cpus
        /// CPUs in which to allow execution (0-3, 0,1)
        /// </summary>
        public DockerRunTask CpusetCpus(string cpusetCpus)
        {
            WithArgumentsValueRequired("--cpuset-cpus", cpusetCpus);
            return this;
        }

        /// <summary>
        /// --cpuset-mems
        /// MEMs in which to allow execution (0-3, 0,1)
        /// </summary>
        public DockerRunTask CpusetMems(string cpusetMems)
        {
            WithArgumentsValueRequired("--cpuset-mems", cpusetMems);
            return this;
        }

        /// <summary>
        /// --cpu-shares, -c
        /// default: 0
        /// CPU shares (relative weight)
        /// </summary>
        public DockerRunTask CpuShares(int cpuShares)
        {
            WithArgumentsValueRequired("--cpu-shares", cpuShares.ToString());
            return this;
        }

        /// <summary>
        /// --detach, -d
        /// Run container in background and print container ID
        /// </summary>
        public DockerRunTask Detach()
        {
            WithArguments("--detach");
            return this;
        }

        /// <summary>
        /// --detach-keys
        /// Override the key sequence for detaching a container
        /// </summary>
        public DockerRunTask DetachKeys(string key)
        {
            WithArgumentsValueRequired("--detach-keys", key);
            return this;
        }

        /// <summary>
        /// --device
        /// Add a host device to the container
        /// </summary>
        public DockerRunTask Device(string device)
        {
            WithArgumentsValueRequired("--device", device);
            return this;
        }

        /// <summary>
        /// --device-cgroup-rule
        /// Add a rule to the cgroup allowed devices list.
        /// </summary>
        public DockerRunTask DeviceCgroupRule(string rule)
        {
            WithArgumentsValueRequired("--device-cgroup-rule", rule);
            return this;
        }

        /// <summary>
        /// --device-read-bps
        /// Limit read rate (bytes per second) from a device
        /// </summary>
        public DockerRunTask DeviceReadBps(ulong deviceReadBps)
        {
            WithArgumentsValueRequired("--device-read-bps", deviceReadBps.ToString());
            return this;
        }

        /// <summary>
        /// --device-read-iops
        /// Limit read rate (IO per second) from a device
        /// </summary>
        public DockerRunTask DeviceReadIops(ulong deviceReadIops)
        {
            WithArgumentsValueRequired("--device-read-iops", deviceReadIops.ToString());
            return this;
        }

        /// <summary>
        /// --device-write-bps
        /// Limit write rate (bytes per second) to a device
        /// </summary>
        public DockerRunTask DeviceWriteBps(ulong deviceWriteBps)
        {
            WithArgumentsValueRequired("--device-write-bps", deviceWriteBps.ToString());
            return this;
        }

        /// <summary>
        /// --device-write-iops
        /// Limit write rate (IO per second) to a device
        /// </summary>
        public DockerRunTask DeviceWriteIops(ulong deviceWriteIops)
        {
            WithArgumentsValueRequired("--device-write-iops", deviceWriteIops.ToString());
            return this;
        }

        /// <summary>
        /// --disable-content-trust
        /// Skip image verification
        /// </summary>
        public DockerRunTask DisableContentTrust()
        {
            WithArguments("--disable-content-trust");
            return this;
        }

        /// <summary>
        /// --dns
        /// Set custom DNS servers.
        /// </summary>
        public DockerRunTask SetDnsServer(string dns)
        {
            WithArgumentsValueRequired("--dns", dns);
            return this;
        }

        /// <summary>
        /// --dns-option
        /// Set DNS options
        /// </summary>
        public DockerRunTask SetDnsOptions(string dnsOption)
        {
            WithArgumentsValueRequired("--dns-option", dnsOption);
            return this;
        }

        /// <summary>
        /// --dns-search
        /// Set custom DNS search domains.
        /// </summary>
        public DockerRunTask SetDnsSearchDomains(string dnsSearch)
        {
            WithArgumentsValueRequired("--dns-search", dnsSearch);
            return this;
        }

        /// <summary>
        /// --entrypoint
        /// Overwrite the default ENTRYPOINT of the image.
        /// </summary>
        public DockerRunTask OverWriteEntrypoint(string entrypoint)
        {
            WithArgumentsValueRequired("--entrypoint", entrypoint);
            return this;
        }

        /// <summary>
        /// --env, -e
        /// Set environment variables
        /// </summary>
        public DockerRunTask SetEnviromentVariables(string env)
        {
            WithArgumentsValueRequired("--env", env);
            return this;
        }

        /// <summary>
        /// --expose
        /// Expose a port or a range of ports
        /// </summary>
        public DockerRunTask ExposePors(string port)
        {
            WithArgumentsValueRequired("--expose", port);
            return this;
        }

        /// <summary>
        /// --group-add
        /// Add additional groups to join
        /// </summary>
        public DockerRunTask GroupAdd(string group)
        {
            WithArgumentsValueRequired("--group-add", group);
            return this;
        }

        /// <summary>
        /// --health-cmd
        /// Command to run to check health
        /// </summary
        public DockerRunTask HealthCmd(string healthCmd)
        {
            WithArgumentsValueRequired("--health-cmd", healthCmd);
            return this;
        }

        /// <summary>
        /// --health-interval
        /// default: 0
        /// Time between running the check (ms|s|m|h) (default 0s)
        /// </summary>
        public DockerRunTask HealthInterval(string interval)
        {
            WithArgumentsValueRequired("--health-interval", interval);
            return this;
        }

        /// <summary>
        /// --health-retries
        /// Consecutive failures needed to report unhealthy. default: 0
        /// </summary>
        public DockerRunTask HealthRetries(uint retries)
        {
            WithArgumentsValueRequired("--health-retries", retries.ToString());
            return this;
        }

        /// <summary>
        /// --health-start-period
        /// Start period for the container to initialize before starting health-retries countdown (ms|s|m|h) (default 0s)
        /// </summary>
        public DockerRunTask HealthStartPeriod(string period)
        {
            WithArgumentsValueRequired("--health-start-period", period);
            return this;
        }

        /// <summary>
        /// --health-timeout
        /// Maximum time to allow one check to run (ms|s|m|h) (default 0s)
        /// </summary>
        public DockerRunTask HealthTimeout(string timeout)
        {
            WithArgumentsValueRequired("--health-timeout", timeout);
            return this;
        }

        /// <summary>
        /// --hostname, -h
        /// Container host name
        /// </summary>
        public DockerRunTask Hostname(string hostname)
        {
            WithArgumentsValueRequired("--hostname", hostname);
            return this;
        }

        /// <summary>
        /// --init
        /// Run an init inside the container that forwards signals and reaps processes.
        /// </summary>
        public DockerRunTask RunInit()
        {
            WithArguments("--init");
            return this;
        }

        /// <summary>
        /// /// <summary>
        /// --interactive, -i
        /// Keep STDIN open even if not attached
        /// </summary>
        /// <returns></returns>
        public DockerRunTask Interactive()
        {
            WithArguments("--interactive");
            return this;
        }

        /// <summary>
        /// --io-maxbandwidth
        /// Maximum IO bandwidth limit for the system drive (Windows only)
        /// </summary>
        public DockerRunTask IoMaxBandwidth(string maxBandwidth)
        {
            WithArgumentsValueRequired("--io-maxbandwidth", maxBandwidth);
            return this;
        }

        public DockerRunTask IoMaxIops(string maxIops)
        {
            WithArgumentsValueRequired("--io-maxiops", maxIops);
            return this;
        }

        /// <summary>
        /// --ip
        /// IPv4 address (e.g., 172.30.100.104)
        /// </summary>
        public DockerRunTask Ip(string ip)
        {
            WithArgumentsValueRequired("--ip", ip);
            return this;
        }

        /// <summary>
        /// --ip6
        /// IPv6 address (e.g., 2001:db8::33)
        /// </summary>
        public DockerRunTask IpV6(string ip)
        {
            WithArgumentsValueRequired("--ip6", ip);
            return this;
        }

        /// <summary>
        /// --ipc
        /// IPC mode to use
        /// </summary>
        public DockerRunTask IpcMode(string ipc)
        {
            WithArgumentsValueRequired("--ipc", ipc);
            return this;
        }

        /// <summary>
        /// --isolation
        /// Container isolation technology
        /// </summary>
        public DockerRunTask IsolationTechnology(string isolation)
        {
            WithArgumentsValueRequired("--isolation", isolation);
            return this;
        }

        /// <summary>
        /// --kernel-memory
        /// Kernel memory limit
        /// </summary>
        public DockerRunTask KernelMemoryLimit(string limit)
        {
            WithArgumentsValueRequired("--kernel-memory", limit);
            return this;
        }

        /// <summary>
        /// --label, -l
        /// Set meta data on a container
        /// </summary>
        public DockerRunTask Label(string label)
        {
            WithArgumentsValueRequired("--label", label);
            return this;
        }

        /// <summary>
        /// --label-file
        /// Read in a line delimited file of labels
        /// </summary>
        public DockerRunTask LabelFile(string labelFile)
        {
            WithArgumentsValueRequired("--label-file", labelFile);
            return this;
        }

        /// <summary>
        /// --link
        /// Add link to another container
        /// </summary>
        public DockerRunTask LinkToAnotherContainer(string link)
        {
            WithArgumentsValueRequired("--link", link);
            return this;
        }

        /// <summary>
        /// --link-local-ip
        /// Container IPv4/IPv6 link-local addresses
        /// </summary>
        public DockerRunTask LinkLocalIp(string ip)
        {
            WithArgumentsValueRequired("--link-local-ip", ip);
            return this;
        }

        /// <summary>
        /// --log-driver
        /// Logging driver for the container
        /// </summary>
        public DockerRunTask LogDriver(string logDriver)
        {
            WithArgumentsValueRequired("--log-driver", logDriver);
            return this;
        }

        /// <summary>
        /// --log-opt
        /// Log driver options
        /// </summary>
        public DockerRunTask LogDriverOptions(string option)
        {
            WithArgumentsValueRequired("--log-opt", option);
            return this;
        }

        /// <summary>
        /// --mac-address
        /// Container MAC address (e.g., 92:d0:c6:0a:29:33)
        /// </summary>
        public DockerRunTask MacAddress(string address)
        {
            WithArgumentsValueRequired("--mac-address", address);
            return this;
        }

        /// <summary>
        /// --memory, -m
        /// Memory limit
        /// </summary>
        public DockerRunTask MemoryLimit(string limit)
        {
            WithArgumentsValueRequired("--memory", limit);
            return this;
        }

        /// <summary>
        /// --memory-reservation
        /// Memory soft limit
        /// </summary>
        public DockerRunTask MemoryReservation(string reservation)
        {
            WithArgumentsValueRequired("--memory-reservation", reservation);
            return this;
        }

        /// <summary>
        /// --memory-swap
        /// Swap limit equal to memory plus swap: &#39;-1&#39; to enable unlimited swap
        /// </summary>
        public DockerRunTask MemorySwap(string memorySwap)
        {
            WithArgumentsValueRequired("--memory-swap", memorySwap);
            return this;
        }

        /// <summary>
        /// --memory-swappiness
        /// Tune container memory swappiness (0 to 100). Default -1.
        /// </summary>
        public DockerRunTask MemorySwappiness(int memorySwappiness)
        {
            WithArgumentsValueRequired("--memory-swappiness", memorySwappiness.ToString());
            return this;
        }

        /// <summary>
        /// --mount
        /// Attach a filesystem mount to the container
        /// </summary>
        public DockerRunTask Mount(string mount)
        {
            WithArgumentsValueRequired("--mount", mount);
            return this;
        }

        /// <summary>
        /// --name
        /// Assign a name to the container
        /// </summary>
        public DockerRunTask ContainerName(string name)
        {
            WithArgumentsValueRequired("--name", name);
            return this;
        }

        /// <summary>
        /// --network
        /// Connect a container to a network. Default: default
        /// </summary>
        public DockerRunTask Network(string network)
        {
            WithArgumentsValueRequired("--network", network);
            return this;
        }

        /// <summary>
        /// --network-alias
        /// Add network-scoped alias for the container
        /// </summary>
        public DockerRunTask NetworkAlias(string networkAlias)
        {
            WithArgumentsValueRequired("--network-alias", networkAlias);
            return this;
        }

        /// <summary>
        /// --no-healthcheck
        /// Disable any container-specified HEALTHCHECK
        /// </summary>
        public DockerRunTask NoHealthCheck()
        {
            WithArguments("--no-healthcheck");
            return this;
        }

        /// <summary>
        /// --oom-kill-disable
        /// Disable OOM Killer
        /// </summary>
        public DockerRunTask OomKillDisable()
        {
            WithArguments("--oom-kill-disable");
            return this;
        }

        /// <summary>
        /// --oom-score-adj
        /// default: 0
        /// Tune host&#39;s OOM preferences (-1000 to 1000)
        /// </summary>
        public DockerRunTask OomScoreAdj(int value)
        {
            WithArgumentsValueRequired("--oom-score-adj", value.ToString());
            return this;
        }

        /// <summary>
        /// --pid
        /// PID namespace to use
        /// </summary>
        public DockerRunTask Pid(string pid)
        {
            WithArgumentsValueRequired("--pid", pid);
            return this;
        }

        /// <summary>
        /// --pids-limit
        /// Tune container pids limit (set -1 for unlimited)
        /// </summary>
        public DockerRunTask PidsLimit(int limit)
        {
            WithArgumentsValueRequired("--pids-limit", limit.ToString());
            return this;
        }

        /// <summary>
        /// --platform
        /// Set platform if server is multi-platform capable
        /// </summary>
        public DockerRunTask Platform(string platform)
        {
            WithArgumentsValueRequired("--platform", platform);
            return this;
        }

        /// <summary>
        /// --privileged
        /// default: false
        /// Give extended privileges to this container
        /// </summary
        public DockerRunTask Privileged()
        {
            WithArguments("--privileged");
            return this;
        }

        /// <summary>
        /// --publish, -p
        /// Publish a container&#39;s port(s) to the host
        /// </summary>
        public DockerRunTask PublishContainerPortToHost(string port)
        {
            WithArgumentsValueRequired("--publish", port);
            return this;
        }

        /// <summary>
        /// --publish-all, -P
        /// default: false
        /// Publish all exposed ports to random ports
        /// </summary>
        public DockerRunTask PublishAllPorts()
        {
            WithArguments("--publish-all");
            return this;
        }

        /// <summary>
        /// --read-only
        /// Mount the container&#39;s root filesystem as read only
        /// </summary>
        public DockerRunTask ReadOnly()
        {
            WithArguments("--read-only");
            return this;
        }

        /// <summary>
        /// --restart
        /// Restart policy to apply when a container exits
        /// </summary
        public DockerRunTask RestartPolicy(string policy)
        {
            WithArgumentsValueRequired("--restart", policy);
            return this;
        }

        /// <summary>
        /// --rm
        /// Automatically remove the container when it exits
        /// </summary>
        public DockerRunTask AutmaticallyRemoveContainer()
        {
            WithArguments("--rm");
            return this;
        }

        /// <summary>
        /// --runtime
        /// Runtime to use for this container
        /// </summary>
        public DockerRunTask Runtime(string runtime)
        {
            WithArgumentsValueRequired("--runtime", runtime);
            return this;
        }

        /// <summary>
        /// --security-opt
        /// Security Options
        /// </summary>
        public DockerRunTask SecurityOptions(string option)
        {
            WithArgumentsValueRequired("--security-opt", option);
            return this;
        }

        /// <summary>
        /// --shm-size
        /// Size of /dev/shm
        /// </summary>
        public DockerRunTask ShmSize(string size)
        {
            WithArgumentsValueRequired("--shm-size", size);
            return this;
        }

        /// <summary>
        /// --sig-proxy
        /// default: true
        /// Proxy received signals to the process
        /// </summary>
        public DockerRunTask SigProxy(string sigProxy)
        {
            WithArguments("--sig-proxy");
            return this;
        }

        /// <summary>
        /// --stop-signal
        /// Signal to stop a container. default: signal.DefaultStopSignal.
        /// </summary>
        public DockerRunTask StopSignal(string signal)
        {
            WithArgumentsValueRequired("--stop-signal", signal);
            return this;
        }

        /// <summary>
        /// --stop-timeout
        /// Timeout (in seconds) to stop a container
        /// </summary>
        public DockerRunTask StopTimeout(uint timeout)
        {
            WithArgumentsValueRequired("--stop-timeout", timeout.ToString());
            return this;
        }

        /// <summary>
        /// --storage-opt
        /// Storage driver options for the container
        /// </summary>
        public DockerRunTask StorageDriverOptions(string option)
        {
            WithArgumentsValueRequired("--storage-opt", option);
            return this;
        }

        /// <summary>
        /// --sysctl
        /// Sysctl options
        /// </summary>
        public DockerRunTask SysctlOptions(string option)
        {
            WithArgumentsValueRequired("--sysctl", option);
            return this;
        }

        /// <summary>
        /// --tmpfs
        /// Mount a tmpfs directory
        /// </summary>
        public DockerRunTask MountTmpfsDirectory(string directory)
        {
            WithArgumentsValueRequired("--tmpfs", directory);
            return this;
        }

        /// <summary>
        /// --tty, -t
        /// Allocate a pseudo-TTY
        /// </summary>
        public DockerRunTask AllocatePsedutoTty()
        {
            WithArguments("--tty");
            return this;
        }

        /// <summary>
        /// --ulimit
        /// Ulimit options
        /// </summary>
        public DockerRunTask UlimitOptions(string option)
        {
            WithArgumentsValueRequired("--ulimit", option);
            return this;
        }

        /// <summary>
        /// --user, -u
        /// Username or UID (format: &lt;name|uid&gt;[:&lt;group|gid&gt;])
        /// </summary>
        public DockerRunTask User(string user)
        {
            WithArgumentsValueRequired("--user", user);
            return this;
        }

        /// <summary>
        /// --userns
        /// User namespace to use
        /// </summary>
        public DockerRunTask UserNamespace(string ns)
        {
            WithArgumentsValueRequired("--userns", ns);
            return this;
        }

        /// <summary>
        /// --uts
        /// UTS namespace to use
        /// </summary>
        public DockerRunTask UtsNamespace(string ns)
        {
            WithArgumentsValueRequired("--uts", ns);
            return this;
        }

        /// <summary>
        /// --volume, -v
        /// Bind mount a volume
        /// </summary>
        public DockerRunTask Volume(string volume)
        {
            WithArgumentsValueRequired("--volume", volume);
            return this;
        }

        /// <summary>
        /// --volume-driver
        /// Optional volume driver for the container
        /// </summary>
        public DockerRunTask VolumeDriver(string volumeDriver)
        {
            WithArgumentsValueRequired("--volume-driver", volumeDriver);
            return this;
        }

        /// <summary>
        /// --volumes-from
        /// Mount volumes from the specified container(s)
        /// </summary>
        public DockerRunTask VolumesFrom(string volumesFrom)
        {
            WithArgumentsValueRequired("--volumes-from", volumesFrom);
            return this;
        }

        /// <summary>
        /// --workdir, -w
        /// Working directory inside the container
        /// </summary>
        public DockerRunTask WorkDir(string workDir)
        {
            WithArgumentsValueRequired("--workdir", workDir);
            return this;
        }

        protected override int DoExecute(ITaskContextInternal context)
        {
            _image.MustNotBeNullOrEmpty("Image must not be null or empty.");

            WithArguments(_image);
            WithArguments(_command);
            WithArguments(_imageArgs);
            return base.DoExecute(context);
        }
    }
}
