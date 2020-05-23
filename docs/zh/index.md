# FlubuCore

[![Travis](https://img.shields.io/travis/dotnetcore/FlubuCore.svg?branch=maste&?style=flat-square&label=build)](https://travis-ci.org/dotnetcore/FlubuCore)
[![NuGet Badge](https://buildstats.info/nuget/flubucore)](https://www.nuget.org/packages/FlubuCore/)
[![Gitter](https://img.shields.io/gitter/room/FlubuCore/Lobby.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Twitter](https://img.shields.io/badge/twitter-flubucore-brightgreen.svg?logo=twitter)](https://twitter.com/FlubuC)
[![Member project of .NET Foundation](https://img.shields.io/badge/.NET-Foundation-68217a.svg)](https://dotnetfoundation.org/projects/flubucore)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![License](https://img.shields.io/github/license/dotnetcore/FlubuCore.svg)](https://github.com/dotnetcore/FlubuCore/blob/master/LICENSE)

## **概述**

“FlubuCore - Fluent Builder Core”，跨平台的构建与部署自动化系统，通过直观的 Fluent 接口，使用 C# 定义构建和部署脚本。这使你的代码获得自动完成、IntelliSense、调试、FlubuCore 自定义分析器，以及在脚本中对整个 .NET 生态的原生性访问。

![FlubuCore in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/demo.gif)

通过 roslyn 的强大赋能，FlubuCore 提供有 .NET/.NET Core 控制台应用程序用于编译和执行脚本。以上示例均可从控制台运行：

- FlubuCore runner `flubu.exe Default`
- FlubuCore dotnet cli tool `dotnet flubu Default`
- FlubuCore local or global tool `flubu Default`

## 功能与优势

- 直观，易学。C#、流畅的 API 设计和 IntelliSense，使复杂脚本的构建变得举重若轻。

```c#
[FromArg("nugetKey", "Nuget api key for publishing Flubu nuget packages.")]
public string NugetApiKey { get; set; }

protected override void ConfigureTargets(ITaskContext context)
{
    var pack = context.CreateTarget("Pack")
        .SetDescription("Prepare's nuget package.")
        .AddCoreTask(x => x.Pack()
            .NoBuild()
            .OutputDirectory(OutputDirectory)
            .WithArguments("--force"); //you can add your own custom arguments on each task

    var branch = context.BuildSystems().Travis().Branch;

    var nugetPush = context.CreateTarget("Nuget.publish")
        .SetDescription("Publishes nuget package.")
        .DependsOn(pack)
        .AddCoreTask(x => x.NugetPush($"{OutputDirectory}/NetCoreOpenSource.nupkg")
            .ServerUrl("https://www.nuget.org/api/v2/package")
             .ApiKey(NugetApiKey)
        )
        .When((c) => c.BuildSystems().RunningOn == BuildSystemType.TravisCI
                     && !string.IsNullOrEmpty(branch)
                     && branch.EndsWith("stable", StringComparison.OrdinalIgnoreCase));
}
```

- [内置大量常用任务](https://flubucore.dotnetcore.xyz/tasks/)，如运行测试、versioning、管理 ISS、创建部署包（deployment packages）、发布 NuGet 包、docker 任务、 sql tasks, git tasks, 执行 PowerShell 脚本等。

```c#
context.CreateTarget("build")
   .AddTask(x => x.GitVersionTask())
   .AddTask(x => x.CompileSolutionTask("MySolution.sln").BuildConfiguration("Release");

context.CreateTarget("run.tests")
   .AddTask(x => x.XunitTaskByProjectName("MyProject").StopOnFail())
   .AddTask(x => x.NUnitTask(NunitCmdOptions.V3, "MyProject2").ExcludeCategory("Linux"))
   .AddCoreTask(x => x.CoverletTask("MyProject.dll"));
```

- [执行自义定代码](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Custom-code)。

```c#
context.CreateTarget("DoExample")
        .Do((c) =>
        {
            //// write your awesome code.
            File.Copy("NotSoAwesome.txt", Path.Combine(OutputDirectory, "JustAnExample.txt") );
            //// Access flubu built in tasks in DO if needed.
            c.Tasks().GenerateT4Template("example.TT").Execute(c);                
        })
        .AddTask(x => x.CompileSolutionTask())
        .Do(NuGetPackageReferencingExample);
```

- 当脚本与项目文件一起使用时[会自动加载程序集引用和 NuGet 包](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Referencing-other-assemblies-in-build-script)。当脚本单独执行（譬如在生产环境中使用 FlubuCore 脚本进行部署）时，可在特性（attributes）中添加引用（references）。

```c#
[NugetPackage("Newtonsoft.json", "11.0.2")]
[Assembly(".\Lib\EntityFramework.dll")]
public class BuildScript : DefaultBuildScript
{
   public void NuGetPackageReferencingExample(ITaskContext context)
    {
        JsonConvert.SerializeObject("Example");
    }
}
```

- [在脚本中轻松运行任何外部程序（external program）或控制台命令（console command）](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Run-any-program)。

```c#
 public class SimpleScript : DefaultBuildScript
 {
	protected override void ConfigureTargets(ITaskContext context)
    {
		context.CreateTarget("Run.Libz")
		.AddTask(x => x.RunProgramTask(@"packages\LibZ.Tool\1.2.0\tools\libz.exe")
			.WorkingFolder(@".\src")
			.WithArguments("add")
			.WithArguments("--libz", "Assemblies.libz"));
	}
 }
```

- [将命令行参数（command line arguments）、json 配置文件或环境变量（environment variables）的设置传入脚本](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Script-arguments)。

```c#
 public class SimpleScript : DefaultBuildScript
 {
    [FromArg("c", "The configuration to use for building the project.")]
    public string Configuration { get; set; } = "Release"
  
    [FromArg("sn", "If true app is deployed on second node. Otherwise not.")]
    public bool deployOnSecondNode { get; set; }
 
    protected override void ConfigureTargets(ITaskContext context)
    {
         context.CreateTarget("build")
            .AddCoreTask(x => x.Build()
                .Configuration(Configuration)
                .ForMember(x =>  x.Framework("net462"), "f", "The target framework to build for.")); 
    }
}
```
 
```
  flubu build -c=Debug -f=netcoreapp2.0
```

- [通过在 FlubuCore 插件中编写自己的任务来扩展 FlubuCore Fluent Api](https://flubucore.dotnetcore.xyz/write-plugins)。

```c#
    public class ExampleFlubuPluginTask : TaskBase<int, ExampleFlubuPluginTask>
    {
        protected override int DoExecute(ITaskContextInternal context)
        {
            // Write your task logic here.
            return 0;
        }
    }
```

- [不断丰富中的 FlubuCore 插件补充着内置任务](https://flubucore.dotnetcore.xyz/AwesomePlugins/awesome-plugins/)。

- [异步执行任务、目标依赖与自定义代码](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Async-execution)。

```c#
    context.CreateTarget("Run.Tests")
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName3"));
```

- [通过控制台程序为任务添加额外配置项（additional options），或对现有的配置项进行重写（override）](https://flubucore.dotnetcore.xyz/override-add-options/)

```c#
context.CreateTarget("Example")`
   .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release");
```

`flubu example --configuration=Debug`

flubu 将执行 `dotnet build MySolution.sln --configuration Debug`

- [完整的 .NET Core 支持，包括全局 CLI 工具](https://flubucore.dotnetcore.xyz/getting-started#getting-started-net-core)

```
    dotnet tool install --global FlubuCore.GlobalTool
    flubu compile
```

- [可对构建脚本测试和调试](https://flubucore.dotnetcore.xyz/Tests-debugging)

```c#
    context.WaitForDebugger();
```

- [透过 FlubuCore Web API 轻松实现远程部署自动化](https://flubucore.dotnetcore.xyz/WebApi/getting-started/)

- [可在其他 .NET 应用程序中使用 FlubuCore 任务](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs)。

- [FlubuCore 交互模式（interactive mode）](https://flubucore.dotnetcore.xyz/build-script-runner-interactive/) 提供有 target 标签自动完成、选项标签自动完成、切换 target/options，以及命令执行历史。还可以执行外部命令和可操作程序。对于其中的一部分，FlubuCore 还提供了 Tab 键自动补全的功能（比如 dotnet、git 等）

![FlubuCore 交互模式](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)

- 使用 FlubuCore 自定义分析器（FlubuCore custom analyzers）增强开发体验。

![执行中的 FlubuCore 分析器](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCoreCustomAnalyzerDemo.png)

## **入门**

FlubuCore 用起来非常简单:-) 而且她的文档也非常完整。

[FlubuCore 文档](https://flubucore.dotnetcore.xyz) 中的[入门](https://flubucore.dotnetcore.xyz/getting-started/)一章将帮助你立即设置你的第一个 FlubuCore 构建。

可在[构建脚本的原理](https://flubucore.dotnetcore.xyz/buildscript-fundamentals) 一章中查阅 FlubuCore 提供的完整功能列表。

一旦你定义了构建与部署脚本（build and deployment scripts），以下 Wiki 张杰将解释如何运行它们：

- 针对 .NET Framework 项目，请使用 [FlubuCore.Runner](https://flubucore.dotnetcore.xyz/getting-started#Installation.net)
- 针对 .NET Core 项目，请使用 [FlubuCore CLI global tool](https://flubucore.dotnetcore.xyz/getting-started#Installation-.net-core)

## **范例**

除了 Wiki 的详细介绍外，FlubuCore 还提供了大量与真实情况相若的范例。这些例子可以在独立仓库 [Examples repository](https://github.com/dotnetcore/FlubuCore.Examples/) 中找到。

这些示例有助于你快速入门 FlubuCore：

这些示例有助于你快速入门 FlubuCore：

- [.NET Framework 构建示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs) - Example covers versioning, building the project, running tests, packaging application for deployment.

- [.NET Core 构建示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs) - Example covers versioning, building the project, running tests, packaging application for deployment.
 
- [部署脚本示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs) - Example shows how to write simple deployment script. 

- [Open source library example](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCoreOpenSource/Build/BuildScript.cs) - Example covers versioning, building the project, running tests and publishing nuget package. It also covers how to run build script on Appveyor and Travis CI.

## **疑惑？**

[![Join the chat at https://gitter.im/FlubuCore/Lobby](https://badges.gitter.im/mbdavid/LiteDB.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## **贡献**

请移步阅读 [CONTRIBUTING.md](https://github.com/dotnetcore/FlubuCore/blob/master/CONTRIBUTING.md).

### **如何作出贡献**

- 为本项目做推广。
- 如果你对本项目感兴趣，请在右上角点击 star，以便壮大我们的社区。
- 改进文档
- 反馈、修正 Bug。
- 实现新功能。
- 讨论如何进一步改进项目。
- 改善项目的现有实现、性能等。

## Further Development
If you find FlubuCore useful (you feel it helps you on the daily basis) you can support further development by buying us a coffee (or become a backer or sponsor). Sometimes it's hard to stay awake till midnight implementing new features, coffee helps us with that. We would really appreciate your support. Money from sponsorship will also be used for the promotion of the project. If you are a backer or a sponsor you can also request for a new feature or ask for support. These issues will be handled with highest priority.

[![](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/NCOpMkt)

### Backers and Sponsors
[![OpenCollective](https://opencollective.com/flubucore/backers/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12502) 
[![OpenCollective](https://opencollective.com/flubucore/sponsors/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12503)

## **更新日志与路线图**

详细变更记录与示例请参阅[变更日志](https://github.com/dotnetcore/FlubuCore/blob/master/CHANGELOG.md)。

FlubuCore 路线图请翻阅项目[里程碑](https://github.com/dotnetcore/FlubuCore/milestones)。

## Acknowledgements

* Special thanks to [@ironcev](https://github.com/ironcev) for greatly improving readme and for giving some valuable advices.
* Special thanks to [@alexinea](https://github.com/https://github.com/alexinea) for translating whole documentation to Chinese.
* Special thanks to [@huanlin](https://github.com/huanlin) for writing blogs about FlubuCore in Traditional Chinese and for translationg them to English.

## .NET Foundation

This project is supported by the [.NET Foundation](http://www.dotnetfoundation.org).
