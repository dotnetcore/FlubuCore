<p align="center">
  <span>English</span> |  
  <a href="https://github.com/dotnetcore/FlubuCore/tree/master/lang/chinese">中文</a>
</p>

# FlubuCore

[![Travis](https://img.shields.io/travis/dotnetcore/FlubuCore.svg?branch=maste&?style=flat-square&label=build)](https://travis-ci.org/dotnetcore/FlubuCore)
[![NuGet Badge](https://buildstats.info/nuget/flubucore)](https://www.nuget.org/packages/FlubuCore/)
[![Gitter](https://img.shields.io/gitter/room/FlubuCore/Lobby.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![License](https://img.shields.io/github/license/dotnetcore/FlubuCore.svg)](https://github.com/dotnetcore/FlubuCore/blob/master/LICENSE)

### Table of Contents

- [Introduction](#Introduction)
- [Features and Advantages](#Features-and-Advantages)
- [Getting Started](#Getting-Started)
- [Examples](#Examples)
- [Contributing](#Contributing)
- [Backers and Sponsors](#Further-Development)
- [Acknowledgements](#Acknowledgements)

## Introduction

"FlubuCore - Fluent Builder Core" is a cross platform build and deployment automation system. You can define your build and deployment scripts in C# using an intuitive fluent interface. This gives you code completion, IntelliSense, debugging, FlubuCore custom analyzers, and native access to the whole .NET ecosystem inside of your scripts.

![FlubuCore in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/demo.gif)

FlubuCore offers a .net (core) console application that uses power of roslyn to compile and execute scripts. Above example can be run from console with:

* FlubuCore runner  ``` flubu.exe Default ```
* FlubuCore dotnet cli tool ``` dotnet flubu Default ```
* FlubuCore global tool ``` flubu Default ```
## Features and Advantages

* Intuitive an easy to learn. C#, fluent interface, and IntelliSense make even most complex script creation a breeze.

```cs
context.CreateTarget("Example")
  .DependsOn(fetchBuildVersionTarget)
  .AddTask(x => x.CompileSolutionTask())
  .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec"))
      .When(c => c.BuildSystems().Jenkins().IsRunningOnJenkins);
```
          
* [Large number of often used built-in tasks](https://flubucore.dotnetcore.xyz/tasks/) like e.g. running tests, managing IIS, creating deployment packages, publishing NuGet packages, docker tasks, executing PowerShell scripts and many more.

```cs
context.CreateTarget("build")
   .AddTask(x => x.GitVersionTask())
   .AddTask(x => x.CompileSolutionTask("MySolution.sln").BuildConfiguration("Release");

context.CreateTarget("run.tests")
   .AddTask(x => x.XunitTaskByProjectName("MyProject").StopOnFail())
   .AddTask(x => x.NUnitTask(NunitCmdOptions.V3, "MyProject2").ExcludeCategory("Linux"))
   .AddCoreTask(x => x.CoverletTask("MyProject.dll"));
```

* [Execute your own custom C# code.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Custom-code)

```cs
context.CreateTarget("MyCustomBuildTarget")
     .AddTask(x => x.CompileSolutionTask())
     .Do(MyCustomMethod)
     .Do(NuGetPackageReferencingExample);
```

* [assembly references and nuget packages are loaded automatically](https://flubucore.dotnetcore.xyz/referencing-external-assemblies/) when script is used together with project file. When script is executed alone (for example when deploying with FlubuCore script on production environment) references can be added with attributes.

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

* [Easily run any external program or console command in your script.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Run-any-program)

```cs
context.CreateTarget("Run.Libz")
    .AddTask(x => x.RunProgramTask(@"packages\LibZ.Tool\1.2.0\tools\libz.exe")
        .WorkingFolder(@".\src")
        .WithArguments("add")
        .WithArguments("--libz", "Assemblies.libz"));
```

* [Pass command line arguments, settings from json configuration file or environment variables to your script.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Script-arguments)

 ```cs
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
* [Extending FlubuCore fluent interface by writing your own tasks within FlubuCore plugins.](https://flubucore.dotnetcore.xyz/write-plugins)

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
* [Growing list of FlubuCore plugins complements built in tasks.](https://flubucore.dotnetcore.xyz/AwesomePlugins/awesome-plugins/)

* [Asynchronous or parallel execution of tasks, target dependencies and custom code.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Async-execution)

    ```cs
    context.CreateTarget("Run.Tests")
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName3"));
    ```

* [Override existing options or add additional options to tasks through console](https://flubucore.dotnetcore.xyz/override-add-options/)

   ```c#
  context.CreateTarget("Example")`
      .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release"); 
   ``` 

  `flubu example --configuration=Debug`

   flubu would execute `dotnet build MySolution.sln --configuration Debug`    

* [Full .NET Core support including the global CLI tool](https://flubucore.dotnetcore.xyz/getting-started#getting-started-net-core)

    ```
    dotnet tool install --global FlubuCore.GlobalTool
    flubu compile
    ```
    
* [FlubuCore interactive mode](https://flubucore.dotnetcore.xyz/build-script-runner-interactive/) which offers target tab completition, options tab completition, toogle targets/options, executed commands history. It is also possible to execute external commands and operable programs. For some of them FlubuCore offers tab completion with help displayed at the bottom of console out of the box(such as dotnet, git..)

![FlubuCore interactive mode](https://raw.githubusercontent.com/dotnetcore/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)

* [Possibility to test and debug your build scripts.](https://flubucore.dotnetcore.xyz/Tests-debugging)

    ```cs
    context.WaitForDebugger();
    ```

* [Easily automate deployments remotely via the FlubuCore Web API.](https://flubucore.dotnetcore.xyz/WebApi/getting-started/)

* [Possibility to use FlubuCore tasks in any other .NET application.](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs)

* Improved developer experience with FlubuCore custom analyzers.

![FlubuCore analyzers in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCoreCustomAnalyzerDemo.png)

## Getting Started
Using FlubuCore is straightforward and very simple :-) It is also fully and throughly documented.

The [Getting Started](https://flubucore.dotnetcore.xyz/getting-started/) chapter in [FlubuCore Documentation](https://flubucore.dotnetcore.xyz) will help you set up your first FlubuCore build in no time.

A comprehensive list of features that FlubuCore has to offer with descriptions can be found in the [Build Script Fundamentals](https://flubucore.dotnetcore.xyz/buildscript-fundamentals) chapter.

Once you have your build and deployment scripts defined, the following Wiki chapters will explain how to run them:
* For .NET Framework projects use [FlubuCore.Runner](https://flubucore.dotnetcore.xyz/getting-started#Installation.net)
* For .NET Core projects use [FlubuCore CLI global tool](https://flubucore.dotnetcore.xyz/getting-started#Installation-.net-core)

## Examples
Aside from the detailed Wiki FlubuCore comes with example projects that reflect real-life situations. The examples can be found in the separate [Examples repository](https://github.com/flubu-core/examples/).

These examples will help you to get quickly start with FlubuCore:
* [.NET Framework build example](https://github.com/flubu-core/examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs
) - Example covers versioning, building the project, running tests, packaging application for deployment.

* [.NET Core build example](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs
) - Example covers versioning, building the project, running tests, packaging application for deployment.

* [Deployment script example](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs
) - Example shows how to write simple deployment script. 

* [Open source library example](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCoreOpenSource/Build/BuildScript.cs) - Example covers versioning, building the project, running tests and publishing nuget package. It also covers how to run build script on Appveyor and Travis CI.
## Have a question?

 [![Join the chat at https://gitter.im/FlubuCore/Lobby](https://badges.gitter.im/mbdavid/LiteDB.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Contributing

Please see [CONTRIBUTING.md](./CONTRIBUTING.md).

### Ways to Contribute

* We appreciate deeply any feedback that you may have! Feel free to participate in the chat, or add an issue in the issue tracker.
* Spread the word about the project.
* If you like the project don't forget to give it a star so that the community get's bigger.
* Improve documentation.
* Report, fix a bug.
* Implement a new feature.
* Discuss potential ways to improve project.
* Improve existing implementation, performance, etc.

## Changelog and Roadmap

Changes with description and examples can be found in [Changelog](https://github.com/flubu-core/flubu.core/blob/master/CHANGELOG.md) 
 
You can see FlubuCore roadmap by exploring opened [Milestones.](https://github.com/flubu-core/flubu.core/milestones)

## Further Development
If you find FlubuCore useful (you feel it helps you on the daily basis) you can support further development by buying us a coffee (or become a backer or sponsor). Sometimes it's hard to stay awake till midnight implementing new features, coffee helps us with that. We would really appreciate your support. Money from sponsorship will also be used for the promotion of the project. If you are a backer or a sponsor you can also request for a new feature or ask for support. These issues will be handled with highest priority.

[![](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/tmAJLYvWy)

### Backers and Sponsors
[![OpenCollective](https://opencollective.com/flubucore/backers/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12502) 
[![OpenCollective](https://opencollective.com/flubucore/sponsors/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12503)

## Used & Powered by
Thank's to Comtrade for supporting us.

[![FlubuCore analyzers in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/Svg/COMTRADE_logo.PNG)](https://www.comtrade.com)

https://www.buymeacoffee.com/NCOpMkt

## Acknowledgements

* Special thanks to [@ironcev](https://github.com/ironcev) for greatly improving readme and for giving some valuable advices.
* Special thanks to [@alexinea](https://github.com/https://github.com/alexinea) for translating whole documentation to Chinese.
