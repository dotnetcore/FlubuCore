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
    }
}
