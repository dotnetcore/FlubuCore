using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using FlubuCore.Scripting;
using Microsoft.DotNet.Cli.Utils;
using Xunit;

namespace Flubu.Tests.Scripting
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
