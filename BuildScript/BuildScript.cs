using System;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Tasks.Nuget;
using System.IO;

public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.CompanyName, "Flubu");
        context.Properties.Set(BuildProps.CompanyCopyright, "Copyright (C) 2010-2016 Flubu");
        context.Properties.Set(BuildProps.ProductId, "FlubuCore");
        context.Properties.Set(BuildProps.ProductName, "FlubuCore");
        context.Properties.Set(BuildProps.BuildDir, "output");
    }

    protected override void ConfigureTargets(ITaskContext context)
    {
        var buildVersion = context.CreateTarget("buildVersion")
            .SetAsHidden()
            .SetDescription("Fetches flubu version from FlubuCore.ProjectVersion.txt file.")
            .AddTask(x => x.FetchBuildVersionFromFileTask());

        var compile = context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "Flubu.Tests/Flubu.Tests.csproj", "FlubuCore.WebApi.Model/FlubuCore.WebApi.Model.csproj", "FlubuCore.WebApi.Client/FlubuCore.WebApi.Client.csproj"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("flubu.sln"))
            .AddCoreTask(x => x.ExecuteDotnetTask("build").WithArguments("flubu.sln"))
            .DependsOn(buildVersion);

        var pack = context.CreateTarget("pack")
            .SetDescription("Packs flubu componets for nuget publishing")
            .AddCoreTask(x => x.ExecuteDotnetTask("pack")
		        .WithArguments("FlubuCore.WebApi.Model", "-c", "Release")
		        .WithArguments("-o", "..\\output"))
	        .AddCoreTask(x => x.ExecuteDotnetTask("pack")
		        .WithArguments("FlubuCore.WebApi.Client", "-c", "Release")
		        .WithArguments("-o", "..\\output"))
			.AddCoreTask(x => x.ExecuteDotnetTask("pack")
                        .WithArguments("FlubuCore", "-c", "Release")
                        .WithArguments("-o", "..\\output"))
            .AddCoreTask(x => x.ExecuteDotnetTask("pack")
                        .WithArguments("dotnet-flubu", "-c", "Release")
                        .WithArguments("-o", "..\\output"))
            .DependsOn(buildVersion);

        var publishWebApi = context.CreateTarget("Publish.WebApi")
            .SetDescription("Publishes flubu web api for deployment")
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi"));

        var packageWebApi = context.CreateTarget("Package.WebApi")
            .SetDescription("Prepares flubu web api deployment package.")
            .AddTask(x => x.PackageTask("output").
             AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp1.1\publish", "FlubuCore.WebApi", true)
            .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
            .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
            .AddFileToPackage("BuildScript\\Deploy.csproj", "")
            .AddFileToPackage("BuildScript\\Deploy.bat", "")
            .AddFileToPackage(@"packages\Newtonsoft.Json.10.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
            .DisableLogging()
            .ZipPackage("FlubuCore.WebApi", true));

        var packageWebApiWin = context.CreateTarget("Package.WebApi.Win")
            .SetDescription("Prepares flubu web api deployment package.")
            .AddTask(x => x.PackageTask("output").
                AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp1.1\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\build.exe", "deploy.exe")
                .AddFileToPackage("output\\build.exe.config", "deploy.exe.config")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.10.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-Win", true));

        var flubuRunnerMerge = context.CreateTarget("merge")
            .SetDescription("Merge's all assemblyes into .net flubu console application")
            .Do(TargetMerge);

	    var flubuTests = context.CreateTarget("test")
		    .SetDescription("Runs all tests in solution.")
		    .AddCoreTask(x => x.ExecuteDotnetTask("test").WithArguments("Flubu.Tests\\Flubu.Tests.csproj"))
		    .AddCoreTask(x => x.ExecuteDotnetTask("test").WithArguments("FlubuCore.WebApi.Tests\\FlubuCore.WebApi.Tests.csproj"));

		var nugetPublish = context.CreateTarget("nuget.publish")
            .Do(PublishNuGetPackage).
            DependsOn(buildVersion);

        var packageFlubuRunner = context.CreateTarget("package.FlubuRunner")
            .SetDescription("Packages .net 4.62 Flubu runner into zip.")
            .Do(TargetPackageFlubuRunner);

        context.CreateTarget("rebuild")
            .SetDescription("Rebuilds the solution")
            .SetAsDefault()
            .DependsOn(compile, flubuTests);

        context.CreateTarget("rebuild.server")
            .SetDescription("Rebuilds the solution and publishes nuget packages.")
            .DependsOn(compile, flubuTests)
            .DependsOn(pack, publishWebApi)
            .DependsOn(flubuRunnerMerge)
            .DependsOn(nugetPublish)
            .DependsOn(packageFlubuRunner)
            .DependsOn(packageWebApi);
            ////.DependsOn(packageWebApiWin);

        var compileLinux = context
            .CreateTarget("compile.linux")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "Flubu.Tests/Flubu.Tests.csproj"))
            .AddCoreTask(x => x.ExecuteDotnetTask("restore").WithArguments("flubu.sln"))
            .DependsOn(buildVersion);

        context.CreateTarget("rebuild.linux")
            .SetDescription("Rebuilds the solution and publishes nuget packages.")
            .DependsOn(compileLinux, flubuTests);
    }

    private static void TargetPackageFlubuRunner(ITaskContext context)
    {
         context.Tasks().PackageTask("output")
            .AddFileToPackage(@"output\build.exe", "flubu.runner")
            .AddFileToPackage(@"output\build.exe.config", "flubu.runner")
            .AddFileToPackage(@"output\flubucore.dll", "flubu.runner")
            .ZipPackage("Flubu runner", true)
            .Execute(context);
    }

    private static void PublishNuGetPackage(ITaskContext context)
    {
        var version = context.Properties.GetBuildVersion();
        var nugetVersion = version.ToString(3);

		try
		{
			context.CoreTasks().ExecuteDotnetTask("nuget")
			 .WithArguments("push")
			 .WithArguments($"output\\FlubuCore.WebApi.Model.{nugetVersion}.nupkg")
			 .WithArguments("-s", "https://www.nuget.org/api/v2/package")
			 .WithArguments("-k", "8da65a4d-9409-4d1b-9759-3b604d7a34ae").Execute(context);
		}
		catch (Exception e)
		{
			Console.WriteLine($"Failed to publish FlubuCore.WebApi.Model. exception: {e.Message}");
		}

		try
		{
			context.CoreTasks().ExecuteDotnetTask("nuget")
			 .WithArguments("push")
			 .WithArguments($"output\\FlubuCore.WebApi.Client.{nugetVersion}.nupkg")
			 .WithArguments("-s", "https://www.nuget.org/api/v2/package")
			 .WithArguments("-k", "8da65a4d-9409-4d1b-9759-3b604d7a34ae").Execute(context);
		}
		catch (Exception e)
		{
			Console.WriteLine($"Failed to publish FlubuCore.WebApi.Client. exception: {e.Message}");
		}

		try
		{
            context.CoreTasks().ExecuteDotnetTask("nuget")
                .WithArguments("push")
                .WithArguments($"output\\FlubuCore.{nugetVersion}.nupkg")
                .WithArguments("-s", "https://www.nuget.org/api/v2/package")
                .WithArguments("-k", "8da65a4d-9409-4d1b-9759-3b604d7a34ae").Execute(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to publish FlubuCore. exception: {e.Message}");
        }

        try
        {
            context.CoreTasks().ExecuteDotnetTask("nuget")
           .WithArguments("push")
           .WithArguments($"output\\dotnet-flubu.{nugetVersion}.nupkg")
           .WithArguments("-s", "https://www.nuget.org/api/v2/package")
           .WithArguments("-k", "8da65a4d-9409-4d1b-9759-3b604d7a34ae").Execute(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to publish dotnet-flubu. exception: {e.Message}");
        }

        try
        {
            var task = context.Tasks().PublishNuGetPackageTask("FlubuCore.Runner", @"Nuget\FlubuCoreRunner.nuspec");
            task.NugetServerUrl("https://www.nuget.org/api/v2/package")
                .ForApiKeyUse("8da65a4d-9409-4d1b-9759-3b604d7a34ae")
                .PushOnInteractiveBuild()
                .Execute(context);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to publish flubu.ruuner. exception: {e}");
        }
    }


    private static void TargetMerge(ITaskContext context)
    {
        var progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net462\win7-x64")
            .WithArguments("add")
            .WithArguments("--libz", "Assemblies.libz")
            .WithArguments("--include", "*.dll")
            .WithArguments("--exclude", "FlubuCore.dll")
            .WithArguments("--move")
            .Execute(context);

        progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net462\win7-x64")
            .WithArguments("inject-libz")
            .WithArguments("--assembly", "dotnet-flubu.exe")
            .WithArguments("--libz", "Assemblies.libz")
            .WithArguments("--move")
            .Execute(context);

        progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net462\win7-x64")
            .WithArguments("instrument")
            .WithArguments("--assembly", "dotnet-flubu.exe")
            .WithArguments("--libz-resources")
            .Execute(context);

        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\dotnet-flubu.exe", @"output\build.exe", true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\dotnet-flubu.exe.config", @"output\build.exe.config", true)
            .Execute(context);

        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\FlubuCore.dll", @"output\FlubuCore.dll", true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\FlubuCore.xml", @"output\FlubuCore.xml", true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\FlubuCore.pdb", @"output\FlubuCore.pdb", true)
            .Execute(context);
    }
}