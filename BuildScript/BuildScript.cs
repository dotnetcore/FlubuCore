using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Context.FluentInterface.Interfaces;
using System.Threading.Tasks;

public class BuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.CompanyName, "Flubu");
        context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2016 Flubu");
        context.Properties.Set(BuildProps.ProductId, "FlubuCore");
        context.Properties.Set(BuildProps.ProductName, "FlubuCore");
        context.Properties.Set(BuildProps.BuildDir, "output");
        context.Properties.Set(BuildProps.SolutionFileName, "flubu.sln");
        context.Properties.Set(BuildProps.BuildConfiguration, "Release");
    }

    protected override void ConfigureTargets(ITaskContext context)
    {
        context.CreateTarget("Test").SetAsDefault()
            .DoAsync(Remedy)
            .DoAsync(Remedy2)
    }

    public async Task Remedy(ITaskContext context)
    {
        context.LogInfo("Beda", ConsoleColor.DarkGray, ConsoleColor.DarkMagenta);
        context.LogInfo("Test", ConsoleColor.Blue);
    }

    public async Task Remedy2(ITaskContext context)
    {
        context.LogInfo("Beda22222222", ConsoleColor.Green, ConsoleColor.Red);
        context.LogInfo("Test2222222222", ConsoleColor.Red);
    }
}