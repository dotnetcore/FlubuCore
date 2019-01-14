using System;
using System.IO;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Context.FluentInterface.Interfaces;

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
        var buildVersion = context.CreateTarget("buildVersion")
            .SetAsHidden()
            .SetDescription("Fetches flubu version from FlubuCore.ProjectVersion.txt file.")
            .AddTask(x => x.FetchBuildVersionFromFileTask());

        var compile = context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "FlubuCore.Tests/FlubuCore.Tests.csproj", "FlubuCore.WebApi.Model/FlubuCore.WebApi.Model.csproj", "FlubuCore.WebApi.Client/FlubuCore.WebApi.Client.csproj", "FlubuCore.WebApi/FlubuCore.WebApi.csproj", "FlubuCore.GlobalTool/FlubuCore.GlobalTool.csproj"))
            .AddCoreTask(x => x.Restore())
            .AddCoreTask(x => x.Build())
            .DependsOn(buildVersion);

        var pack = context.CreateTarget("pack")
            .SetDescription("Packs flubu componets for nuget publishing")
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.WebApi.Model").IncludeSymbols()
                .OutputDirectory("..\\output"))
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.WebApi.Client").IncludeSymbols()
                .OutputDirectory("..\\output"))
            .AddCoreTask(x => x.Pack().IncludeSymbols()
                .Project("FlubuCore")
                .OutputDirectory("..\\output"))
            .AddCoreTask(x => x.Pack()
                .Project("dotnet-flubu").IncludeSymbols()
                .OutputDirectory("..\\output"))
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.GlobalTool").IncludeSymbols()
                .OutputDirectory("..\\output"))
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.Analyzers").IncludeSymbols()
                .OutputDirectory("..\\output"))
            .DependsOn(buildVersion);

        var publishWebApi = context.CreateTarget("Publish.WebApi")
            .SetDescription("Publishes flubu web api for deployment")
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("netcoreapp2.1"))
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("netcoreapp2.0"))
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("netcoreapp1.1"))
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("net462"));

        var packageWebApi = context.CreateTarget("Package.WebApi")
                .AddTasks(PackageWebApi);

        var flubuRunnerMerge = context.CreateTarget("merge")
            .SetDescription("Merge's all assemblyes into .net flubu console application")
            .Do(TargetMerge);

        var flubuTests = context.CreateTarget("test")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.Test().Project("FlubuCore.Tests\\FlubuCore.Tests.csproj"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.WebApi.Tests\\FlubuCore.WebApi.Tests.csproj"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.Analyzers.Tests\\FlubuCore.Analyzers.Tests.csproj"));

        var nugetPublish = context.CreateTarget("nuget.publish")
            .Do(PublishNuGetPackage).
            DependsOn(buildVersion);

        var packageFlubuRunner = context.CreateTarget("package.FlubuRunner")
            .SetDescription("Packages .net 4.62 Flubu runner into zip.")
            .Do(TargetPackageFlubuRunner);

        var packageDotnetFlubu = context.CreateTarget("package.DotnetFlubu")
            .SetDescription("Packages dotnet-flubu tool into zip.")
            .Do(TargetPackageDotnetFlubu);

        context.CreateTarget("rebuild")
            .SetDescription("Rebuilds the solution")
            .SetAsDefault()
            .DependsOn(compile, flubuTests);

        context.CreateTarget("rebuild.server")
            .SetDescription("Rebuilds the solution and publishes nuget packages.")
            .DependsOn(compile, flubuTests)
            .DependsOnAsync(pack, publishWebApi)
            .DependsOn(flubuRunnerMerge)
            .DependsOn(packageFlubuRunner)
            .DependsOn(packageDotnetFlubu)
            .DependsOn(packageWebApi);
            ////.DependsOn(packageWebApiWin);

        var compileLinux = context
            .CreateTarget("compile.linux")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "FlubuCore.Tests/FlubuCore.Tests.csproj", "FlubuCore.GlobalTool/FlubuCore.GlobalTool.csproj"))
            .AddCoreTask(x => x.Restore())
            .DependsOn(buildVersion);

        var flubuTestsLinux = context.CreateTarget("test.linux")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.Test().Project("FlubuCore.Tests\\FlubuCore.Tests.csproj").WithArguments("--filter", "Category!=OnlyWindows"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.WebApi.Tests\\FlubuCore.WebApi.Tests.csproj"));

        context.CreateTarget("rebuild.linux")
            .SetDescription("Rebuilds the solution and publishes nuget packages.")
            .DependsOn(compileLinux, flubuTestsLinux, packageDotnetFlubu);
    }

    private static void TargetPackageFlubuRunner(ITaskContext context)
    {
         context.Tasks().PackageTask("output")
            .AddFileToPackage(@"output\flubu.exe", "flubu.runner")
            .AddFileToPackage(@"output\flubu.exe.config", "flubu.runner")
            .AddFileToPackage(@"output\flubucore.dll", "flubu.runner")
            .ZipPackage("Flubu runner", true)
            .Execute(context);
    }

    private static void TargetPackageDotnetFlubu(ITaskContext context)
    {
        context.CoreTasks().Publish("dotnet-flubu").Framework("netcoreapp2.0").Execute(context);
        if (!Directory.Exists(@"output/dotnet-flubu"))
        {
            Directory.CreateDirectory(@"output/dotnet-flubu");
        }

        context.Tasks().PackageTask(@"output/dotnet-flubu")
            .AddDirectoryToPackage(@"dotnet-flubu/bin/Release/netcoreapp2.0/publish", "", true)
            .ZipPackage("dotnet-flubu", true)
            .Execute(context);
    }

    private static void PublishNuGetPackage(ITaskContext context)
    {
        var version = context.Properties.GetBuildVersion();
        var nugetVersion = version.ToString(3);

        var key = context.ScriptArgs["nugetKey"];

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.WebApi.Model. exception: {e.Message}"); })
            .WithArguments("push")
            .WithArguments($"output\\FlubuCore.WebApi.Model.{nugetVersion}.nupkg")
            .WithArguments("-s", "https://www.nuget.org/api/v2/package")
            .WithArguments("-k", key).Execute(context);

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.WebApi.Client. exception: {e.Message}"); })
            .WithArguments("push")
            .WithArguments($"output\\FlubuCore.WebApi.Client.{nugetVersion}.nupkg")
            .WithArguments("-s", "https://www.nuget.org/api/v2/package")
            .WithArguments("-k", key).Execute(context);

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore. exception: {e.Message}"); })
            .WithArguments("push")
            .WithArguments($"output\\FlubuCore.{nugetVersion}.nupkg")
            .WithArguments("-s", "https://www.nuget.org/api/v2/package")
            .WithArguments("-k", key).Execute(context);

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish dotnet-flubu. exception: {e.Message}"); })
            .WithArguments("push")
            .WithArguments($"output\\dotnet-flubu.{nugetVersion}.nupkg")
            .WithArguments("-s", "https://www.nuget.org/api/v2/package")
            .WithArguments("-k", key).Execute(context);

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.GlobalTool. exception: {e.Message}"); })
            .WithArguments("push")
            .WithArguments($"output\\FlubuCore.GlobalTool.{nugetVersion}.nupkg")
            .WithArguments("-s", "https://www.nuget.org/api/v2/package")
            .WithArguments("-k", key).Execute(context);

        context.CoreTasks().ExecuteDotnetTask("nuget")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.Analyzer. exception: {e.Message}"); })
            .WithArguments("push")
            .WithArguments($"output\\FlubuCore.Analyzers.1.0.3.nupkg")
            .WithArguments("-s", "https://www.nuget.org/api/v2/package")
            .WithArguments("-k", key).Execute(context);

        var task = context.Tasks().PublishNuGetPackageTask("FlubuCore.Runner", @"Nuget\FlubuCoreRunner.nuspec");
        task.NugetServerUrl("https://www.nuget.org/api/v2/package")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish flubu.ruuner. exception: {e}"); })
            .ForApiKeyUse(key)
            .Execute(context);
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
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\dotnet-flubu.exe", @"output\flubu.exe", true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\win7-x64\dotnet-flubu.exe.config", @"output\flubu.exe.config", true)
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

    public static void PackageWebApi(ITarget target)
    {
        target.SetDescription("Prepares flubu web api deployment package.")
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\net462\win7-x64\publish", "FlubuCore.WebApi",
                    true)
                .AddFileToPackage("BuildScript\\DeployScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\flubu.exe", "")
                .AddFileToPackage("output\\flubu.exe.config", "")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-Net462", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.0\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\flubu.exe", "")
                .AddFileToPackage("output\\flubu.exe.config", "")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.1\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\flubu.exe", "")
                .AddFileToPackage("output\\flubu.exe.config", "")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.1-WindowsInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.0\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("BuildScript\\NetCore2.0\\Deploy.csproj", "")
                .AddFileToPackage("BuildScript\\Deploy.bat", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.1\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("BuildScript\\NetCore2.0\\Deploy.csproj", "")
                .AddFileToPackage("BuildScript\\Deploy.bat", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.1-LinuxMacInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp1.1\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\flubu.exe", "")
                .AddFileToPackage("output\\flubu.exe.config", "")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard1.3\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp1.1-WindowsInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp1.1\publish", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("BuildScript\\NetCore1.1\\Deploy.csproj", "")
                .AddFileToPackage("BuildScript\\Deploy.bat", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json.11.0.2\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard1.3\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp1.1-LinuxMacInstaller", true));
    }
}