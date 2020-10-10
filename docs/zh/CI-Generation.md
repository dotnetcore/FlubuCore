#### feature is in beta. Available from FlubuCore 6.0.0-preview3)**

Most of the times builds are run on contionous integration environment. With FlubuCore configuration files for Azure pipelines, Github Actions, Appveyor, Travis and Jenkins can be generated.

Configuration files can be generated with command `flubu {targetName} --ci={CIName}`. Available CINames: Jenkins, AppVeyor, Travis, Azure, GithubActions. Seperate them with comma when generating multiple configuration files 

### **Practical sample - how to generate CI configuration file from FlubuCore build script**

 Let's say we have the following build script and we want to integrate our build script into azure pipelines.
 

```c#
public class BuildScript : DefaultBuildScript
{
    [SolutionFileName] public string SolutionFileName { get; set; } = "flubu.sln";

    [BuildConfiguration] public string BuildConfiguration { get; set; } = "Release";

    protected override void ConfigureTargets(ITaskContext context)
    {
        var buildVersion = context.CreateTarget("buildVersion")
            .SetAsHidden()
            .SetDescription("Fetches flubu version from CHANGELOG.md file.")
            .AddTask(x => x.FetchBuildVersionFromFileTask()
                .ProjectVersionFileName("../CHANGELOG.md"));

        var compile = context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.UpdateNetCoreVersionTask("FlubuCore/FlubuCore.csproj",
                "dotnet-flubu/dotnet-flubu.csproj", "FlubuCore.Tests/FlubuCore.Tests.csproj",
                "FlubuCore.WebApi.Model/FlubuCore.WebApi.Model.csproj",
                "FlubuCore.WebApi.Client/FlubuCore.WebApi.Client.csproj", "FlubuCore.WebApi/FlubuCore.WebApi.csproj",
                "FlubuCore.GlobalTool/FlubuCore.GlobalTool.csproj"))
            .AddCoreTask(x => x.Build())
            .DependsOn(buildVersion);

        var tests = context.CreateTarget("test")
            .SetDescription("Runs all tests in solution.")
            .AddCoreTask(x => x.Test().Project("FlubuCore.Tests\\FlubuCore.Tests.csproj"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.WebApi.Tests\\FlubuCore.WebApi.Tests.csproj"))
            .AddCoreTask(x => x.Test().Project("FlubuCore.Analyzers.Tests\\FlubuCore.Analyzers.Tests.csproj"));

        context.CreateTarget("rebuild")
            .SetDescription("Rebuilds the solution")
            .SetAsDefault()
            .DependsOn(compile, tests);
    }
}
```
 
 We can generate Azure pipelines configuration file with command `flubu Rebuild --ci=Azure`. This command would generate Azure.Generated.yaml file with the following content:
 
```yaml
jobs:
- job: windows_latest
  pool:
    vmimage: windows-latest
  steps:
  - task: DotNetCoreInstaller@1
    displayName: Install .net core sdk
    inputs:
      version: 3.1.302
  - script: dotnet tool install --global FlubuCore.Tool --version 5.1.8
    displayName: Install flubu
  - script: flubu buildVersion --nd
    displayName: buildVersion
  - script: flubu compile --nd
    displayName: compile
  - script: flubu test --nd
    displayName: test
- job: ubuntu_latest
  pool:
    vmimage: ubuntu-latest
  steps:
  - task: DotNetCoreInstaller@1
    displayName: Install .net core sdk
    inputs:
      version: 3.1.302
  - script: dotnet tool install --global FlubuCore.Tool --version 5.1.8
    displayName: Install flubu
  - script: flubu buildVersion --nd
    displayName: buildVersion
  - script: flubu compile --nd
    displayName: compile
  - script: flubu test --nd
    displayName: test
- job: macOs_latest
  pool:
    vmimage: macOs-latest
  steps:
  - task: DotNetCoreInstaller@1
    displayName: Install .net core sdk
    inputs:
      version: 3.1.302
  - script: dotnet tool install --global FlubuCore.Tool --version 5.1.8
    displayName: Install flubu
  - script: flubu buildVersion --nd
    displayName: buildVersion
  - script: flubu compile --nd
    displayName: compile
  - script: flubu test --nd
    displayName: test
```

### **Customizing configuration settings in CI configuration files.** 

Alot of times additional configuration settings needs to be added in configuration file. Such as adding additional steps before or after flubu target execution, setting on which virtual machine image build is executed, installing additional services that are needed for build execution, setting working directory etc.
This can be achived in two ways. 

1. Generated configuration file is edited manually after it is generated with flubu. Meaning generated file is used as a template and all additional configuration settings are added manually to the file.

2. Additional configuration settings are added through fluent api in Configure method. All supported CI configuration files can be customized completly through fluent api. We wont go into details about the 
   fluent api as we think it is self explainatory and easy to learn.
   

```c#
public class BuildScript : DefaultBuildScript
{
    public override void Configure(IFlubuConfigurationBuilder configurationBuilder, ILoggerFactory loggerFactory)
    {
        configurationBuilder.ConfigureAppVeyor(o => o
            .SetCloneDepth(50)
            .SetVirtualMachineImage(AppVeyorImage.VisualStudio2019, AppVeyorImage.Ubuntu1804)
            .AddSkipCommits("docs/*", "assets/*", "LICENCE", "mkdocs.yml", "README.md", ".travis.yml", ".gitignore",
                "lang/*")
            .SetWorkingDirectory("src")
            .AddArtifacts("output/*.zip")
            .AddCustomTarget("Rebuild.Linux", AppVeyorImage.Ubuntu1804));

        configurationBuilder.ConfigureTravis(t => t
            .AddOs(TravisOs.Linux)
            .SetDotnetVersion("3.1.201")
            .AddServices("docker")
            .AddBeforeScript("sudo apt-get install dotnet-sdk-2.2")
            .AddBeforeScript("cd src"));

        configurationBuilder.ConfigureAzurePipelines(az => az
            .SetWorkingDirectory("src")
            .CustomTargetsForVmImage(AzurePipelinesImage.UbuntuLatest,
                "Rebuild.Linux") ////  /// specified target(s) is used for flubu script generation. Script is applied only to specified image and target specified in command line is ignored for specified image. can be applied to all other CI suported servers.
            .CustomTargetsForVmImage(AzurePipelinesImage.MacOsLatest, "Rebuild.MacOs"));


        configurationBuilder.ConfigureGitHubActions(gh => gh
            .SetWorkingDirectory("src")
            .OnPullRequest().AddBranches("master, develop")
            .AddCustomStepBeforeTargets(s =>
            {
                s.Name = "Clean"; ////Just a dummy example that a custom step can be added before(or after) flubu target steps. Same applies to all other CI suported servers.
                s.Run = "dotnet clean";
            }));
    }
}
```