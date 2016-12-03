using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Cli.Flubu;
using DotNet.Cli.Flubu.Infrastructure;
using DotNet.Cli.Flubu.Scripting;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Extensions;
using FlubuCore.IO.Wrappers;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using FlubuCore.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Flubu.Tests
{
    public class SystemTests
    {
        [Fact]
        public void ExecuteMvcNet4_61BuildScript()
        {
            Assert.Equal(0, Program.Main(new string[] { "Rebuild", @"-s=.\FlubuExamples\MVC_NET4.61\BuildScriptTest.cs" }));
        }
    }
}
