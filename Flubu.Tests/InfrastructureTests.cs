using System;
using DotNet.Cli.Flubu.Commanding;
using DotNet.Cli.Flubu.Infrastructure;
using FlubuCore;
using FlubuCore.Commanding;
using FlubuCore.Tasks;
using FlubuCore.Tasks.Process;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Flubu.Tests
{
    public class InfrastructureTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        private readonly IServiceProvider _provider;

        public InfrastructureTests()
        {
            _services
                .AddCoreComponents()
                .AddScriptAnalyser()
                .AddCommandComponents()
                .AddArguments(new string[] { })
                .AddTasks();

            _provider = _services.BuildServiceProvider();
        }

        [Fact]
        public void ResolveCommandExecutor()
        {
            var executor = _provider.GetRequiredService<ICommandExecutor>();

            Assert.IsType<CommandExecutor>(executor);
        }

        [Fact]
        public void ResolveRunProgramTask()
        {
            IServiceCollection sc = new ServiceCollection();
            sc.AddTransient<ICommandFactory, CommandFactory>();

            IServiceProvider pr = sc.BuildServiceProvider();
            IRunProgramTask instance = ActivatorUtilities.CreateInstance<RunProgramTask>(pr, "test");
            Assert.IsType<RunProgramTask>(instance);
        }

        [Fact]
        public void ExampleTestExtensions()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(
                @"using System;
                using System.Collections;
                using System.Linq;
                using System.Text;

                namespace HelloWorld
                {
                class Program
                {
                    static void Main(string[] args)
                    {
                        Console.WriteLine(""Hello, World!"");
                    }
                }
                }");

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var firstMember = root.Members[0];

            var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;

            var programDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];

            var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];

            var argsParameter = mainDeclaration.ParameterList.Parameters[0];
        }
    }
}