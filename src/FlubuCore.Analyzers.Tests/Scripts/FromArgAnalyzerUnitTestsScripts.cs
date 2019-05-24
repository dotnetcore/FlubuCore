using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Analyzers.Tests.Scripts
{
    public static class FromArgAnalyzerUnitTestsScripts
    {
        public const string FromArgSupportedPropertyTypeScript = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
   public class FromArgAttribute : Attribute
    {
        public FromArgAttribute(string argKey, string help = null)
        {
            ArgKey = argKey;
            Help = help;
        }

        public string ArgKey { get; }

        public string Help { get; }
    }

    public class SimpleScript : DefaultBuildScript
    {

        [FromArg(""t"")]
        public List<string> Test { get; set; }

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }
    }
}";

        public const string FromArgNotSupportedPropertyTypeScript = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
   public class FromArgAttribute : Attribute
    {
        public FromArgAttribute(string argKey, string help = null)
        {
            ArgKey = argKey;
            Help = help;
        }

        public string ArgKey { get; }

        public string Help { get; }
    }

    public class SimpleScript : DefaultBuildScript
    {

        [FromArg(""t"")]
        public object Test { get; set; }

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }
    }
}";

        public const string FromArgKeyValueShouldNotStartWithDashScript = @"
using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
   public class FromArgAttribute : Attribute
    {
        public FromArgAttribute(string argKey, string help = null)
        {
            ArgKey = argKey;
            Help = help;
        }

        public string ArgKey { get; }

        public string Help { get; }
    }

    public class SimpleScript : DefaultBuildScript
    {

        [FromArg(""-t"")]
        public bool Test { get; set; }

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }
    }
}";
    }
}
