using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore.Analyzers.Tests.Scripts
{
    public static class TargetParameterAnalyzerUnitTestsScripts
    {
        public const string CorrectTargetDefinititionScript = @"
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
   public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"", ""SomeFile"")]
        public void SuccesfullTarget(ITarget target, string fileName)
        {
        }
     }
}";

        public const string WrongFirstParameterScript = @"
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
    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(\""Test\"")]
        public void SuccesfullTarget(string fileName)
        {
        }
     }
}";

        public const string NoFirstParameterScript = @"
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
    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target((""Test"")]
        public void SuccesfullTarget()
        {
        }
     }
}";

        public const string WrongParameterCountScript = @"using System;
            using System.IO;
        using FlubuCore.Context;
        using FlubuCore.Context.FluentInterface;
        using FlubuCore.Context.FluentInterface.Interfaces;
        using FlubuCore.Scripting;
        using FlubuCore.Targeting;
        using Moq;

        namespace FlubuCore.WebApi.Tests
        {
        public class Target : System.Attribute
        {
            public TargetAttribute(string targetName, params object[] methodParameters)
            {
                TargetName = targetName;
                MethodParameters = methodParameters;
            }

            public string TargetName { get; private set; }

            public object[] MethodParameters { get; set; }
        }

        public class SimpleScript : DefaultBuildScript
        {
            protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
            {
            }

            protected override void ConfigureTargets(ITaskContext session)
            {
            }

            [Target(""Test"", ""param1"", ""param2"", ""param3"")]
            public void SuccesfullTarget(ITarget target, string fileName, string path)
            {
            }
        }
    }";

        public const string AttributeDoesntHaveParametersMethodDoesScript = @"using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"")]
        public void SuccesfullTarget(ITarget target, string fileName)
        {
        }
     }
}";

        public const string AttributeAndMethodParameterCountAreTheSameScript = @"using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"" , ""someFilename"")]
        public void SuccesfullTarget(ITarget target, string fileName)
        {
        }
     }
}";

        public const string WrongParameterType = @"using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Context.FluentInterface;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.Scripting;
using FlubuCore.Targeting;
using Moq;

namespace FlubuCore.WebApi.Tests
{
    public class Target : System.Attribute
    {
        public TargetAttribute(string targetName, params object[] methodParameters)
        {
            TargetName = targetName;
            MethodParameters = methodParameters;
        }

        public string TargetName { get; private set; }

        public object[] MethodParameters { get; set; }
    }

    public class SimpleScript : DefaultBuildScript
    {
        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

        protected override void ConfigureTargets(ITaskContext session)
        {
        }

        [Target(""Test"", ""param1"", 1)]
        public void SuccesfullTarget(ITarget target, string fileName, string path)
        {
        }
     }
}";
    }
}
