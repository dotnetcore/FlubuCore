using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlubuCore.Context;
using FlubuCore.Scripting;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.CompanyName, "Flubu");
        context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2016 Flubu");
        context.Properties.Set(BuildProps.ProductId, "FlubuCoreExample");
        context.Properties.Set(BuildProps.ProductName, "FlubuCoreExample");
    }

    protected override void ConfigureTargets(ITaskContext context)
    {
        context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("FlubuExample"))
            .TaskExtensions().DotnetBuild("FlubuExample");

    } 
}