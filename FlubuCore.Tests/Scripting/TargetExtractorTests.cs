using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Scripting;
using Xunit;

namespace FlubuCore.Tests.Scripting
{
    public class TargetExtractorTests
    {
        private readonly ITargetExtractor _targetExtractor;

        public TargetExtractorTests()
        {
            _targetExtractor = new TargetExtractor();
        }

        [Theory]
        [InlineData("var buildVersion = context.CreateTarget(\"buildVersion\")", "buildVersion")]
        [InlineData("            .CreateTarget(\"compile\")", "compile")]
        [InlineData("", null)]
        [InlineData(null, null)]
        [InlineData(".createTarget(\"compile\")", null)]
        public void ExtractTargetTest(string line, string expectedTargetName)
        {
            var targetName = _targetExtractor.ExtractTarget(line);
            Assert.Equal(expectedTargetName, targetName);
        }

        [Fact]
        public void ExtractTargetsTest()
        {
            var scriptLines = new List<string>
            {
                "using System;",
                "using FlubuCore.Context;",
                "using FlubuCore.Scripting;",
                string.Empty,
                "public class MyBuildScript : DefaultBuildScript",
                "{",
                "    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)",
                "    {",
                "        System.Console.WriteLine(\"2222\");",
                "        }",
                string.Empty,
                "        protected override void ConfigureTargets(ITaskContext context)",
                "        {",
                "            var test = new Test();",
                "    context.CreateTarget(\"rebuild.server\")",
                " .SetDescription(\"Rebuilds the solution and publishes nuget packages.\")",
                ".DependsOn(compile, flubuTests);",
                "session.CreateTarget(\"Deploy\")",
                "    .SetDescription(\"Deploys flubu web api\")",
                "context.CreateTarget(\"rebuild.linux\")",
                "        }",
                "    }",
                "public class Test",
                "{",
                "}"
            };

            var result = _targetExtractor.ExtractTargets(scriptLines);

            Assert.Equal(3, result.Count);
            Assert.Equal("rebuild.server", result[0]);
            Assert.Equal("Deploy", result[1]);
            Assert.Equal("rebuild.linux", result[2]);
        }
    }
}
