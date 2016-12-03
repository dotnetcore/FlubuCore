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
        private readonly IFileWrapper _fileLoader = new FileWrapper();

        private readonly ScriptLoader _loader;

        public SystemTests()
        {
            _loader = new ScriptLoader(_fileLoader);
        }

        [Fact]
        public void ExecuteMvcNet4_61BuildScript()
        {
            Program.Main(new string[] { "Rebuild", @"-s=.\FlubuExamples\MVC_NET4.61\BuildScriptTest.cs" });
        }
    }
}
