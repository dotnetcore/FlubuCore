<p align="center">
  <a href="https://github.com/dotnetcore/FlubuCore">English</a> | 
  <span>中文</span>  
</p>

# FlubuCore

[![Build Status](https://travis-ci.org/dotnetcore/FlubuCore.svg?branch=master)](https://travis-ci.org/dotnetcore/FlubuCore)
[![NuGet Badge](https://buildstats.info/nuget/flubucore)](https://www.nuget.org/packages/FlubuCore/)
[![Gitter](https://img.shields.io/gitter/room/FlubuCore/Lobby.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Twitter](https://img.shields.io/badge/twitter-flubucore-brightgreen.svg?logo=twitter)](https://twitter.com/FlubuC)
[![Member project of .NET Foundation](https://img.shields.io/badge/.NET-Foundation-68217a.svg)](https://dotnetfoundation.org/projects/flubucore)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![License](https://img.shields.io/github/license/dotnetcore/FlubuCore.svg)](https://github.com/dotnetcore/FlubuCore/blob/master/LICENSE)

- [介绍](#介绍)
- [功能与优势](#功能与优势)
- [入门](#入门)
- [范例](#范例)
- [贡献](#贡献)
- [支持者和赞助商](#支持者和赞助商)
- [致谢](#致谢)

## 介绍

“FlubuCore - Fluent Builder Core”，跨平台的构建与部署自动化系统，通过直观的 Fluent 接口，使用 C# 定义构建和部署脚本。这使你的代码获得自动完成、IntelliSense、调试、FlubuCore 自定义分析器，以及在脚本中对整个 .NET 生态的原生性访问。

![FlubuCore in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/demo.gif)

通过 roslyn 的强大赋能，FlubuCore 提供有 .NET/.NET Core 控制台应用程序用于编译和执行脚本。以上示例均可从控制台运行：

- FlubuCore runner (.NET 4.62+)  `flubu.exe Default`
- FlubuCore dotnet cli tool (.NET Core 1.0+)  `dotnet flubu Default`
- FlubuCore local or global tool (.NET Core 2.1+) `flubu Default`

## 功能与优势

- 直观，易学。C#、流畅的 API 设计和 IntelliSense，使复杂脚本的构建变得举重若轻。

```cs
[FromArg("nugetKey", "Nuget api key for publishing Flubu nuget packages.")]
public string NugetApiKey { get; set; }

protected override void ConfigureTargets(ITaskContext context)
{
    var pack = context.CreateTarget("Pack")
        .SetDescription("Prepare's nuget package.")
        .AddCoreTask(x => x.Pack()
            .NoBuild()
            .OutputDirectory(OutputDirectory)
            .WithArguments("--force")); //you can add your own custom arguments on each task

    var branch = context.BuildSystems().Travis().Branch;
  
    var nugetPush = context.CreateTarget("Nuget.publish")
        .SetDescription("Publishes nuget package.")
        .DependsOn(pack)
        .AddCoreTask(x => x.NugetPush($"{OutputDirectory}/NetCoreOpenSource.nupkg")
            .ServerUrl("https://www.nuget.org/api/v2/package")
            .ApiKey(NugetApiKey)
        )
        .When(c => c.BuildSystems().RunningOn == BuildSystemType.TravisCI
                    && !string.IsNullOrEmpty(branch)
                    && branch.EndsWith("stable", StringComparison.OrdinalIgnoreCase));
}
```

- [内置大量常用任务](https://flubucore.dotnetcore.xyz/tasks/)，如运行测试、versioning、管理 IIS、创建部署包（deployment packages）、发布 NuGet 包、docker 任务、 sql tasks, git tasks, 执行 PowerShell 脚本等。

```cs
context.CreateTarget("build")
   .AddTask(x => x.GitVersionTask())
   .AddTask(x => x.CompileSolutionTask("MySolution.sln").BuildConfiguration("Release");

context.CreateTarget("run.tests")
   .AddTask(x => x.XunitTaskByProjectName("MyProject").StopOnFail())
   .AddTask(x => x.NUnitTask(NunitCmdOptions.V3, "MyProject2").ExcludeCategory("Linux"))
   .AddCoreTask(x => x.CoverletTask("MyProject.dll"));
```

- [执行自义定代码](https://flubucore-zh.dotnetcore.xyz/buildscript-fundamentals#Custom-code)。

```cs
context.CreateTarget("DoExample")
        .Do(c =>
        {
            // write your awesome code.
            File.Copy("NotSoAwesome.txt", Path.Combine(OutputDirectory, "JustAnExample.txt") );
            // Access flubu built in tasks in DO if needed.
            c.Tasks().GenerateT4Template("example.TT").Execute(c);                
        })
        .AddTask(x => x.CompileSolutionTask())
        .Do(NuGetPackageReferencingExample);
```

- 当脚本与项目文件一起使用时[会自动加载程序集引用和 NuGet 包](https://flubucore-zh.dotnetcore.xyz/referencing-external-assemblies/)。当脚本单独执行（譬如在生产环境中使用 FlubuCore 脚本进行部署）时，可在特性（attributes）中添加引用（references）。

```cs
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

- [在脚本中轻松运行任何外部程序（external program）或控制台命令（console command）](https://flubucore-zh.dotnetcore.xyz/buildscript-fundamentals#Run-any-program)。

```cs
context.CreateTarget("Run.Libz")
    .AddTask(x => x.RunProgramTask(@"packages\LibZ.Tool\1.2.0\tools\libz.exe")
        .WorkingFolder(@".\src")
        .WithArguments("add")
        .WithArguments("--libz", "Assemblies.libz"));
```

- [将命令行参数（command line arguments）、json 配置文件或环境变量（environment variables）的设置传入脚本](https://flubucore-zh.dotnetcore.xyz/buildscript-fundamentals#Script-arguments)。

```cs
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

- [通过在 FlubuCore 插件中编写自己的任务来扩展 FlubuCore Fluent Api](https://flubucore-zh.dotnetcore.xyz/write-plugins)。

```cs
public class ExampleFlubuPluginTask : TaskBase<int, ExampleFlubuPluginTask>
{
    protected override int DoExecute(ITaskContextInternal context)
    {
        // Write your task logic here.
        return 0;
    }
}
```

- [不断丰富中的 FlubuCore 插件补充着内置任务](https://flubucore-zh.dotnetcore.xyz/AwesomePlugins/awesome-plugins/)。

- [异步执行任务、目标依赖与自定义代码](https://flubucore-zh.dotnetcore.xyz/buildscript-fundamentals#Async-execution)。

```cs
context.CreateTarget("Run.Tests")
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName3"));
```

- [通过控制台程序为任务添加额外配置项（additional options），或对现有的配置项进行重写（override）](https://flubucore-zh.dotnetcore.xyz/override-add-options/)

```c#
context.CreateTarget("Example")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release");
```

`flubu example --configuration=Debug`

flubu 将执行 `dotnet build MySolution.sln -c Debug`

- [完整的 .NET Core 支持，包括本地或全局 CLI 工具](https://flubucore-zh.dotnetcore.xyz/getting-started#getting-started-net-core)

```
dotnet tool install --global FlubuCore.Tool
flubu compile
```

- [可对构建脚本测试和调试](https://flubucore-zh.dotnetcore.xyz/Tests-debugging)

```cs
context.WaitForDebugger();
```

* [为 Azure pipelines、Github actions、Appveyor、Travis 和 Jenkins 生成持续集成配置文件](https://flubucore.dotnetcore.xyz/CI-Generation/) 

- [透过 FlubuCore Web API 轻松实现远程部署自动化](https://flubucore-zh.dotnetcore.xyz/WebApi/getting-started/)

- [可在其他 .NET 应用程序中使用 FlubuCore 任务](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs)。

- [FlubuCore 交互模式（interactive mode）](https://flubucore-zh.dotnetcore.xyz/build-script-runner-interactive/) 提供有 target 标签自动完成、选项标签自动完成、切换 target 和选项，以及命令执行历史等。

![FlubuCore 交互模式](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)

- 使用 FlubuCore 自定义分析器（FlubuCore custom analyzers）增强开发体验。

![执行中的 FlubuCore 分析器](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCoreCustomAnalyzerDemo.png)

## 入门

FlubuCore 用起来非常简单:-) 而且她的文档也非常完整。

[FlubuCore 文档](https://flubucore-zh.dotnetcore.xyz) 中的[入门](https://flubucore.dotnetcore.xyz/getting-started/)一章将帮助你立即设置你的第一个 FlubuCore 构建。

可在[构建脚本的原理](https://flubucore-zh.dotnetcore.xyz/buildscript-fundamentals) 一章中查阅 FlubuCore 提供的完整功能列表。

一旦你定义了构建与部署脚本（build and deployment scripts），以下 Wiki 章节将解释如何运行它们：

- 针对 .NET Framework 项目，请使用 [FlubuCore.Runner](https://flubucore-zh.dotnetcore.xyz/getting-started#Installation.net)
- 针对 .NET Core 项目，请使用 [FlubuCore CLI global tool](https://flubucore-zh.dotnetcore.xyz/getting-started#Installation-.net-core)

## 范例

除了 Wiki 的详细介绍外，FlubuCore 还提供了大量与真实情况相若的范例。这些例子可以在独立仓库 [Examples repository](https://github.com/dotnetcore/FlubuCore.Examples/) 中找到。

这些示例有助于你快速入门 FlubuCore：

- [.NET Framework 构建示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs) - 示例包括版本控制、构建项目、运行测试、打包用于部署的应用程序以及其他一些基本用例。

- [.NET Core 构建示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs) - 示例包括版本控制、构建项目、运行测试、打包用于部署的应用程序以及其他一些基本用例。
 
- [部署脚本示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs) - 示例展示了如何编写简单的部署脚本。 

- [开源项目示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCoreOpenSource/Build/BuildScript.cs) - 示例包括版本控制、构建项目、运行测试和发布 nuget 包。它还包括如何通过 Appveyor 和 Travis CI 运行构建脚本。

## 疑惑？

[![Join the chat at https://gitter.im/FlubuCore/Lobby](https://badges.gitter.im/mbdavid/LiteDB.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## 贡献

请移步阅读 [CONTRIBUTING.md](./CONTRIBUTING.md).

### 如何作出贡献

- 为本项目做推广。
- 如果你对本项目感兴趣，请在右上角点击 star，以便壮大我们的社区。
- 改进文档
- 反馈、修正 Bug。
- 实现新功能。
- 讨论如何进一步改进项目。
- 改善项目的现有实现、性能等。

## 更新日志与路线图

详细变更记录与示例请参阅[变更日志](https://github.com/dotnetcore/FlubuCore/blob/master/CHANGELOG.md)。

FlubuCore 路线图请翻阅项目[里程碑](https://github.com/dotnetcore/FlubuCore/milestones)。

## 后续发展

如果您发现 FlubuCore 有用（您认为它每天都对你有帮助），可以请我们喝一杯咖啡（或成为支持者或赞助商）来支持进一步的开发工作。 有时很难保持清醒，直到午夜实现新功能，咖啡可以帮助我们实现这一目标。 非常感谢您的支持。 赞助资金也将用于项目推广。 如果您是支持者或赞助者，则还可以请求新功能特性或获取帮助。 这些问题将得到最高优先级处理。

[![](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/NCOpMkt)

### 支持者和赞助商

[![OpenCollective](https://opencollective.com/flubucore/backers/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12502) 
[![OpenCollective](https://opencollective.com/flubucore/sponsors/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12503)

## 使用与支持

感谢 Comtrade 对我们的支持。

[![FlubuCore analyzers in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/Svg/COMTRADE_logo.PNG)](https://www.comtrade.com)

## 致谢

- 特别感谢 [@ironcev](https://github.com/ironcev) 对 readme 文件做了大量改进并提供了一系列富有价值的建议。
- 特别感谢 [@alexinea](https://github.com/alexinea) 将整个文档翻译成中文。
- 特别感谢 [@huanlin](https://github.com/huanlin) 用繁体中文撰写有关 FlubuCore 的博客，并翻译成英文。

## Code of Conduct

这个项目采用了 [Code of Conduct](http://contributor-covenant.org/) 定义的行为准则阐明我们社区中的预期行为。
有关更多信息，请参见 [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct) 。

## .NET Foundation

该项目受 [.NET Foundation](http://www.dotnetfoundation.org) 支持。
