using System.Collections.Generic;
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
            var watch = System.Diagnostics.Stopwatch.StartNew();
            resolver.ResolveNugetPackages(new List<NugetPackageReference>()
            {
                new NugetPackageReference
                {
                    Id = "FlubuCore",
                    Version = "2.8.0",
                }
            }, null);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
        }
    }
}
