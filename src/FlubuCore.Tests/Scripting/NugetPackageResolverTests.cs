using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Services;
using Microsoft.DotNet.Cli.Utils;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class NugetPackageResolverTests
    {
        [Fact(Skip = "Explicit test as it needs connection to nuget.")]
        public void ResolveNugetDependencies()
        {
            var resolver = new NugetPackageResolver(new CommandFactory(), new FileWrapper(), new FlubuEnviromentService());

            var assemblies = resolver.ResolveNugetPackagesFromDirectives(new List<NugetPackageReference>()
            {
                new NugetPackageReference
                {
                    Id = "FlubuCore",
                    Version = "2.8.0",
                },
                new NugetPackageReference()
                {
                    Id = "Dapper",
                    Version = "1.50.5"
                },
            }, null);

            Assert.True(assemblies.Count > 100);
            Assert.Contains(assemblies, x => x.Name == "FlubuCore");
            Assert.Contains(assemblies, x => x.Name == "Dapper");
        }
    }
}
