using System.Collections.Generic;
using System.Linq;
using FlubuCore.Scripting;
using Microsoft.DotNet.Cli.Utils;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class NugetPackageResolverTests
    {
        [Fact]
        public void Resolve()
        {
            var resolver = new NugetPackageResolver(new CommandFactory());
            var assemblies = resolver.ResolveNugetPackages(new List<NugetPackageReference>()
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
            Assert.True(assemblies.Any(x => x.Name == "FlubuCore"));
            Assert.True(assemblies.Any(x => x.Name == "Dapper"));
        }
    }
}
