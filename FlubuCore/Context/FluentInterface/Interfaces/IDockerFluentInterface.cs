using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Docker;

namespace FlubuCore.Context.FluentInterface.Interfaces
{
    public interface IDockerFluentInterface
    {
        /// <summary>
        /// The docker build command builds Docker images from a Dockerfile and a “context”. A build’s context is the set of files located in the specified PATH or URL.
        /// </summary>
        /// <param name="pathOrUrl">Path or url to a build’s context set of files.</param>
        /// <returns></returns>
        DockerBuildTask Build(string pathOrUrl);

        /// <summary>
        /// Docker runs processes in isolated containers. A container is a process which runs on a host.
        /// The host may be local or remote. When an operator executes docker run, the container process that runs is isolated in that it has its own file system, its own networking, and its own isolated process tree separate from the host.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="command"></param>
        /// <param name="imageArgs"></param>
        /// <returns></returns>
        DockerRunTask Run(string image, string command, params string[] imageArgs);

        /// <summary>
        /// Stop one or more running containers.
        /// </summary>
        /// <param name="containers"></param>
        /// <returns></returns>
        DockerStopTask Stop(params string[] containers);

        /// <summary>
        /// Remove one or more containers.
        /// </summary>
        /// <param name="containers"></param>
        /// <returns></returns>
        DockerRemoveContainerTask RemoveContainer(params string[] containers);
    }
}
