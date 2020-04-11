using System;
using System.IO;
using FlubuCore.Commanding;
using FlubuCore.Context;
using FlubuCore.Scripting;
using FlubuCore.Context.FluentInterface.Interfaces;
using FlubuCore.IO;
using FlubuCore.Context.Attributes.BuildProperties;

public class  BuildScript : DefaultBuildScript
{
    [FromArg("nugetKey", "Nuget api key for publishing Flubu nuget packages.")]
    public string NugetApiKey { get; set; }

    public FullPath Output => RootDirectory.CombineWith("Output");

    [ProductId] public string ProductId { get; set; } = "FlubuCore";

    [SolutionFileName] public string SolutionFileName { get; set; } = "flubu.sln";

    [BuildConfiguration] public string BuildConfiguration { get; set; } = "Release";

    protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
        context.Properties.Set(BuildProps.BuildDir, "output");

    }

    protected override void ConfigureTargets(ITaskContext context)
    {
        var buildVersion = context.CreateTarget("buildVersion")
            .SetAsHidden()
            .SetDescription("Fetches flubu version from CHANGELOG.md file.")
            .AddTask(x => x.FetchBuildVersionFromFileTask()
                .ProjectVersionFileName("../CHANGELOG.md")
                .RemovePrefix("## FlubuCore ")
                .RemovePrefix("## FlubuCore"));

        var compile = context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "FlubuCore.Tests/FlubuCore.Tests.csproj", "FlubuCore.WebApi.Model/FlubuCore.WebApi.Model.csproj", "FlubuCore.WebApi.Client/FlubuCore.WebApi.Client.csproj", "FlubuCore.WebApi/FlubuCore.WebApi.csproj", "FlubuCore.GlobalTool/FlubuCore.GlobalTool.csproj"))
            .AddCoreTask(x => x.Build())
            .DependsOn(buildVersion);

        var pack = context.CreateTarget("pack")
            .SetDescription("Packs flubu componets for nuget publishing.")
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.WebApi.Model").IncludeSymbols()
                .OutputDirectory(Output))
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.WebApi.Client").IncludeSymbols()
                .OutputDirectory(Output))
            .AddCoreTask(x => x.Pack().IncludeSymbols()
                .Project("FlubuCore")
                .OutputDirectory(Output))
            .AddCoreTask(x => x.Pack()
                .Project("dotnet-flubu").IncludeSymbols()
                .OutputDirectory(Output))
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.GlobalTool").IncludeSymbols()
                .OutputDirectory(Output))
            .AddCoreTask(x => x.Pack()
                .Project("FlubuCore.Analyzers").IncludeSymbols()
                .OutputDirectory(Output))
            .DependsOn(buildVersion);

        var publishWebApi = context.CreateTarget("Publish.WebApi")
            .SetDescription("Publishes flubu web api for deployment")
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("netcoreapp2.1"))
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("netcoreapp2.0"))
            .AddCoreTask(x => x.Publish("FlubuCore.WebApi").Framework("net462"));
        

        var packageWebApi = context.CreateTarget("Package.WebApi")
            .SetDescription("Packages web api into zip deployment package.")
                .AddTasks(PackageWebApi);

        var flubuRunnerMerge = context.CreateTarget("merge")
            .SetDescription("Merge's all assemblyes into .net flubu console application executable.")
            .Do(TargetMerge);

        var flubuTests = context.CreateTarget("test")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.Test().Project("FlubuCore.Tests\\FlubuCore.Tests.csproj"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.WebApi.Tests\\FlubuCore.WebApi.Tests.csproj"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.Analyzers.Tests\\FlubuCore.Analyzers.Tests.csproj"));

        var nugetPublish = context.CreateTarget("nuget.publish")
            .SetDescription("Publishes all FlubuCore nuget packages.")
            .Do(PublishNuGetPackage).
            DependsOn(buildVersion);

        var packageFlubuRunner = context.CreateTarget("package.FlubuRunner")
            .SetDescription("Packages .net 4.62 FlubuCore runner into zip.")
            .Do(TargetPackageFlubuRunner);

        var packageDotnetFlubu = context.CreateTarget("package.DotnetFlubu")
            .SetDescription("Packages dotnet-flubu tool into zip.")
            .Do(TargetPackageDotnetFlubu);

        context.CreateTarget("rebuild")
            .SetDescription("Rebuilds the solution")
            .SetAsDefault()
            .DependsOn(compile, flubuTests);

        var branch = context.BuildSystems().AppVeyor().BranchName;
        
        context.CreateTarget("rebuild.server")
            .SetDescription("Rebuilds the solution and publishes nuget packages.")
            .SequentialLogging(true)
            .DependsOn(compile, flubuTests)
            .DependsOn(pack, publishWebApi)
            .DependsOnAsync(flubuRunnerMerge)
            .DependsOn(packageFlubuRunner)
            .DependsOn(packageDotnetFlubu)
            .DependsOn(packageWebApi)
            .DependsOn(nugetPublish).When((c) =>
                c.BuildSystems().RunningOn == BuildSystemType.AppVeyor && branch != null && branch.Contains("stable", StringComparison.OrdinalIgnoreCase));
            ////.DependsOn(packageWebApiWin);

        var compileLinux = context
            .CreateTarget("compile.linux")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj", "dotnet-flubu/dotnet-flubu.csproj", "FlubuCore.Tests/FlubuCore.Tests.csproj", "FlubuCore.GlobalTool/FlubuCore.GlobalTool.csproj"))
            .AddCoreTask(x => x.Restore())
            .DependsOn(buildVersion);

        var flubuTestsLinux = context.CreateTarget("test.linux")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.Test().Project("FlubuCore.Tests\\FlubuCore.Tests.csproj").AddFilter("Category!=OnlyWindows"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.WebApi.Tests\\FlubuCore.WebApi.Tests.csproj"));

        context.CreateTarget("rebuild.linux")
            .SetDescription("Rebuilds the solution.")
            .DependsOn(compileLinux, flubuTestsLinux, packageDotnetFlubu);
    }

    private void TargetPackageFlubuRunner(ITaskContext context)
    {
         context.Tasks().PackageTask("output")
            .AddFileToPackage(Output.CombineWith("flubu.exe"), "flubu.runner")
            .AddFileToPackage(Output.CombineWith("flubu.exe.config"), "flubu.runner")
            .AddFileToPackage(Output.CombineWith("flubucore.dll"), "flubu.runner")
            .ZipPackage("Flubu runner", true)
            .Execute(context);
    }

    private void TargetPackageDotnetFlubu(ITaskContext context)
    {
        context.CoreTasks().Publish("dotnet-flubu").Framework("netcoreapp2.0").Execute(context);
        if (!Directory.Exists(Output.CombineWith("dotnet-flubu")))
        {
            Directory.CreateDirectory(@"output/dotnet-flubu");
        }

        context.Tasks().PackageTask(Output.CombineWith("dotnet-flubu"))
            .AddDirectoryToPackage(@"dotnet-flubu/bin/Release/netcoreapp2.0/publish", "", true)
            .ZipPackage("dotnet-flubu", true)
            .Execute(context);
    }

    private  void PublishNuGetPackage(ITaskContext context)
    {
        var version = context.Properties.GetBuildVersion();
        var nugetVersion = version.ToString(3);
        var versionQuality = context.Properties.GetBuildVersionQuality();

        if (!string.IsNullOrEmpty(versionQuality))
        {
            nugetVersion = $"{nugetVersion}-{versionQuality}";
        }
        
        context.CoreTasks().NugetPush(Output.CombineWith($"FlubuCore.WebApi.Model.{nugetVersion}.nupkg"))
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.WebApi.Model. exception: {e.Message}"); })
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey).Execute(context);

        context.CoreTasks().NugetPush(Output.CombineWith($"FlubuCore.WebApi.Client.{nugetVersion}.nupkg"))
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.WebApi.Client. exception: {e.Message}"); })
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey).Execute(context);

        context.CoreTasks().NugetPush(Output.CombineWith($"FlubuCore.{nugetVersion}.nupkg"))
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore. exception: {e.Message}"); })
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey).Execute(context);

        context.CoreTasks().NugetPush(Output.CombineWith($"dotnet-flubu.{nugetVersion}.nupkg"))
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish dotnet-flubu. exception: {e.Message}"); })
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey).Execute(context);

        context.CoreTasks().NugetPush(Output.CombineWith($"FlubuCore.GlobalTool.{nugetVersion}.nupkg"))
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.GlobalTool. exception: {e.Message}"); })
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey).Execute(context);

        context.CoreTasks().NugetPush(Output.CombineWith("FlubuCore.Analyzers.1.0.4.nupkg"))
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish FlubuCore.Analyzer. exception: {e.Message}"); })
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey).Execute(context);

        var task = context.Tasks().PublishNuGetPackageTask("FlubuCore.Runner", @"Nuget\FlubuCoreRunner.nuspec");
        task.NugetServerUrl("https://www.nuget.org/api/v2/package")
            .DoNotFailOnError(e => { Console.WriteLine($"Failed to publish flubu.ruuner. exception: {e}"); })
            .ForApiKeyUse(NugetApiKey)
            .Execute(context);
    }

    private void TargetMerge(ITaskContext context)
    {
        var progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net462")
            .WithArguments("add")
            .WithArguments("--libz", "Assemblies.libz")
            .WithArguments("--include", "*.dll")
            .WithArguments("--exclude", "FlubuCore.dll")
            .WithArguments("--move")
            .Execute(context);
        
        progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net462")
            .WithArguments("inject-libz")
            .WithArguments("--assembly", "dotnet-flubu.exe")
            .WithArguments("--libz", "Assemblies.libz")
            .WithArguments("--move")
            .Execute(context);

        progTask = context.Tasks().RunProgramTask(@"tools\LibZ.Tool\1.2.0\tools\libz.exe");

        progTask
            .WorkingFolder(@"dotnet-flubu\bin\Release\net462")
            .WithArguments("instrument")
            .WithArguments("--assembly", "dotnet-flubu.exe")
            .WithArguments("--libz-resources")
            .Execute(context);

        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\dotnet-flubu.exe", Output.CombineWith("flubu.exe"), true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\dotnet-flubu.exe.config", Output.CombineWith("flubu.exe.config"), true)
            .Execute(context);

        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\FlubuCore.dll", Output.CombineWith("FlubuCore.dll"), true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\\FlubuCore.xml", Output.CombineWith("FlubuCore.xml"), true)
            .Execute(context);
        context.Tasks()
            .CopyFileTask(@"dotnet-flubu\bin\Release\net462\FlubuCore.pdb", Output.CombineWith("FlubuCore.pdb"), true)
            .Execute(context);
    }

    public void PackageWebApi(ITarget target)
    {
        target.SetDescription("Prepares flubu web api deployment package.")
            .AddTask(x => x.PackageTask(Output.CombineWith("WebApiPackages"))
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\net462\publish", "FlubuCore.WebApi", true)
                .AddDirectoryToPackage(@"FlubuCore.WebApi.Updater\bin\Release\net462", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeployScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage(Output.CombineWith("flubu.exe"), "")
                .AddFileToPackage(Output.CombineWith("flubu.exe.config"), "")
                .AddFileToPackage(Output.CombineWith("FlubuCore.dll"), "")
                .AddFileToPackage(@"packages\Newtonsoft.Json\12.0.3\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-Net462", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.0\publish", "FlubuCore.WebApi", true)
                .AddDirectoryToPackage(@"FlubuCore.WebApi.Updater\bin\Release\netcoreapp2.0", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\flubu.exe", "")
                .AddFileToPackage("output\\flubu.exe.config", "")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json\12.0.3\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.0-WindowsInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.1\publish", "FlubuCore.WebApi", true)
                .AddDirectoryToPackage(@"FlubuCore.WebApi.Updater\bin\Release\netcoreapp2.0", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("output\\flubu.exe", "")
                .AddFileToPackage("output\\flubu.exe.config", "")
                .AddFileToPackage("output\\FlubuCore.dll", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json\12.0.3\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.1-WindowsInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.0\publish", "FlubuCore.WebApi", true)
                .AddDirectoryToPackage(@"FlubuCore.WebApi.Updater\bin\Release\netcoreapp2.0", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("BuildScript\\NetCore2.0\\Deploy.csproj", "")
                .AddFileToPackage("BuildScript\\Deploy.bat", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json\12.0.3\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.0-LinuxMacInstaller", true))
            .AddTask(x => x.PackageTask("output\\WebApiPackages")
                .AddDirectoryToPackage(@"FlubuCore.WebApi\bin\Release\netcoreapp2.1\publish", "FlubuCore.WebApi", true)
                .AddDirectoryToPackage(@"FlubuCore.WebApi.Updater\bin\Release\netcoreapp2.0", "FlubuCore.WebApi", true)
                .AddFileToPackage("BuildScript\\DeploymentScript.cs", "")
                .AddFileToPackage("BuildScript\\DeploymentConfig.json", "")
                .AddFileToPackage("BuildScript\\NetCore2.0\\Deploy.csproj", "")
                .AddFileToPackage("BuildScript\\Deploy.bat", "")
                .AddFileToPackage(@"packages\Newtonsoft.Json\12.0.3\lib\netstandard1.3\Newtonsoft.Json.dll", "lib")
                .AddFileToPackage(@"packages\litedb\4.1.2\lib\netstandard2.0\LiteDB.dll", "lib")
                .AddFileToPackage(@"packages\System.Reflection.TypeExtensions.dll", "lib")
                .AddFileToPackage(@"packages\System.Security.Cryptography.Algorithms.dll", "lib")
                .AddFileToPackage(@"packages\netstandard.dll", "lib")
                .DisableLogging()
                .ZipPackage("FlubuCore.WebApi-NetCoreApp2.1-LinuxMacInstaller", true));
    }
}