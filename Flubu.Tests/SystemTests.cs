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
        public const string BaseExamplesPath = @".\FlubuExamples\";

        [Fact]
        public void ExecuteMvcNet4_61BuildScript()
        {
            string buildScriptArg = @"-s=" + BaseExamplesPath + @"MVC_NET4.61\BuildScriptTest.cs";
            Assert.Equal(0, Program.Main(new string[] { "Rebuild", buildScriptArg, "-tte=compile,Rebuild,Sample,load.solution" }));
        }

        [Fact]
        public void ExecuteMvcNetCore1_0BuildScript()
        {
            string buildScriptArg = @"-s=" + BaseExamplesPath + @"NetCore_1.1\BuildScript\BuildScriptTest.cs";
            Assert.Equal(0, Program.Main(new string[] { "compile", buildScriptArg, "-tte=compile" }));
        }
    }
}
