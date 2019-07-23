# FlubuCore

[![Windows Build](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore)](http://lucidlynx.comtrade.com:8080/login?from=%2F)
[![Travis](https://img.shields.io/travis/dotnetcore/FlubuCore.svg?branch=maste&?style=flat-square&label=linux-build)](https://travis-ci.org/dotnetcore/FlubuCore)
[![NuGet](https://img.shields.io/nuget/v/FlubuCore.svg)](https://www.nuget.org/packages/FlubuCore)
[![Gitter](https://img.shields.io/gitter/room/FlubuCore/Lobby.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Twitter](https://img.shields.io/badge/twitter-flubucore-brightgreen.svg?logo=twitter)](https://twitter.com/FlubuC)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![License](https://img.shields.io/github/license/dotnetcore/FlubuCore.svg)](https://github.com/dotnetcore/FlubuCore/blob/master/LICENSE)

## **概述**

“FlubuCore - Fluent Builder Core”，跨平台的构建与部署自动化系统，通过直观的 Fluent 接口，使用 C# 定义构建和部署脚本。这使你的代码获得自动完成、IntelliSense、调试、FlubuCore 自定义分析器，以及在脚本中对整个 .NET 生态的原生性访问。

![FlubuCore in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/demo.gif)

通过 roslyn 的强大赋能，FlubuCore 提供有 .NET/.NET Core 控制台应用程序用于编译和执行脚本。以上示例均可从控制台运行：

- FlubuCore runner `flubu.exe Default`
- FlubuCore dotnet cli tool `dotnet flubu Default`
- FlubuCore global tool `flubu Default`

## 功能与优势

- 直观，易学。C#、流畅的 API 设计和 IntelliSense，使复杂脚本的构建变得举重若轻。

```c#
context.CreateTarget("Example")
  .DependsOn(fetchBuildVersionTarget)
  .AddTask(x => x.CompileSolutionTask())
  .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec"))
      .When(c => c.BuildSystems().Jenkins().IsRunningOnJenkins);
```

- [内置大量常用任务](https://flubucore.dotnetcore.xyz/tasks/)，如运行测试、管理 ISS、创建部署包（deployment packages）、发布 NuGet 包、docker 任务、执行 PowerShell 脚本等。

```c#
target
    .AddTask(x => x.CompileSolutionTask())
    .AddTask(x => x.CopyFileTask(source, destination, true))
    .AddTask(x => x.IisTasks()
                    .CreateAppPoolTask("Example app pool")
                    .Mode(CreateApplicationPoolMode.DoNothingIfExists));
```

- [执行自义定代码](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Custom-code)。

```c#
context.CreateTarget("MyCustomBuildTarget")
     .AddTask(x => x.CompileSolutionTask())
     .Do(MyCustomMethod)
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
    [FromArg("sn", "If true app is deployed on second node. Otherwise not.")]
    public bool deployOnSecondNode { get; set; }


     protected override void ConfigureTargets(ITaskContext context)
     {
         context.CreateTarget("compile")
            .AddTask(x => x.CompileSolutionTask()
                .ForMember(y => y.SolutionFileName("someSolution.sln"), "solution", "The solution to build."));
     }
 }
```

```
  flubu.exe compile -solution=someOtherSolution.sln -sn=true
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

`flubu build /o:configuration=Debug`

flubu 将执行 `dotnet build MySolution.sln -c Debug`

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

- [FlubuCore 交互模式（interactive mode）](https://flubucore.dotnetcore.xyz/build-script-runner-interactive/) 提供有 target 标签自动完成、选项标签自动完成、切换 target 和选项，以及命令执行历史等。

![FlubuCore 交互模式](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode.gif)

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

- [.NET Framework 构建示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs)

- [.NET Core 构建示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs)

- [部署脚本示例](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs)

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

## **更新日志与路线图**

详细变更记录与示例请参阅[变更日志](https://github.com/dotnetcore/FlubuCore/blob/master/CHANGELOG.md)。

FlubuCore 路线图请翻阅项目[里程碑](https://github.com/dotnetcore/FlubuCore/milestones)。
