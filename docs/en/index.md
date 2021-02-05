# FlubuCore
[![Travis](https://img.shields.io/travis/dotnetcore/FlubuCore.svg?branch=maste&?style=flat-square&label=build)](https://travis-ci.org/dotnetcore/FlubuCore)
[![NuGet Badge](https://buildstats.info/nuget/flubucore)](https://www.nuget.org/packages/FlubuCore/)
[![Gitter](https://img.shields.io/gitter/room/FlubuCore/Lobby.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Twitter](https://img.shields.io/badge/twitter-flubucore-brightgreen.svg?logo=twitter)](https://twitter.com/FlubuC)
[![Member project of .NET Foundation](https://img.shields.io/badge/.NET-Foundation-68217a.svg)](https://dotnetfoundation.org/projects/flubucore)
[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![License](https://img.shields.io/github/license/dotnetcore/FlubuCore.svg)](https://github.com/dotnetcore/FlubuCore/blob/master/LICENSE)

## **Introduction**
"FlubuCore - Fluent Builder Core" is a cross platform build and deployment automation system. You can define your build and deployment scripts in C# using an intuitive fluent interface. This gives you code completion, IntelliSense, debugging, FlubuCore custom analyzers, and native access to the whole .NET ecosystem inside of your scripts.

![FlubuCore in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/demo.gif)

FlubuCore offers a .net (core) console application that uses power of roslyn to compile and execute scripts. Above example can be run from console with:

* FlubuCore runner  ``` flubu.exe Default ```
* FlubuCore dotnet cli tool ``` dotnet flubu Default ```
* FlubuCore local or global tool ``` flubu Default ```

## **Features and Advantages**

* Intuitive an easy to learn. C#, fluent interface, and IntelliSense make even most complex script creation a breeze.

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

    //// Examine travis.yaml to see how to pass api key from travis to FlubuCore build script.
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
          
* [Large number of often used built-in tasks](https://flubucore.dotnetcore.xyz/tasks/) like e.g. versioning, running tests, creating deployment packages, publishing NuGet packages, docker tasks, git tasts, sql tasks, npm tasks, executing PowerShell, managing IIS scripts and many more.

```c#
context.CreateTarget("build")
   .AddTask(x => x.GitVersionTask())
   .AddTask(x => x.CompileSolutionTask("MySolution.sln").BuildConfiguration("Release");

context.CreateTarget("run.tests")
   .AddTask(x => x.XunitTaskByProjectName("MyProject").StopOnFail())
   .AddTask(x => x.NUnitTask(NunitCmdOptions.V3, "MyProject2").ExcludeCategory("Linux"))
   .AddCoreTask(x => x.CoverletTask("MyProject.dll"));
```

* [Execute your own custom C# code.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Custom-code)

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

* [assembly references and nuget packages are loaded automatically](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Referencing-other-assemblies-in-build-script) when script is used together with project file. When script is executed alone (for example when deploying with FlubuCore script on production environment) references can be added with attributes.

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

* [Easily run any external program or console command in your script.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Run-any-program)

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

* [Pass command line arguments, settings from json configuration file or environment variables to your script.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Script-arguments)

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

* [Extending FlubuCore fluent interface by writing your own tasks within FlubuCore plugins.](https://flubucore.dotnetcore.xyz/write-plugins)

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

* [Growing list of FlubuCore plugins complements built in tasks.](https://flubucore.dotnetcore.xyz/AwesomePlugins/awesome-plugins/)

* [Asynchronous execution of tasks, target dependencies and custom code.](https://flubucore.dotnetcore.xyz/buildscript-fundamentals#Async-execution)

```c#
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

* [Possibility to test and debug your build scripts.](https://flubucore.dotnetcore.xyz/Tests-debugging)

```c#
    context.WaitForDebugger();
```

* [Easily automate deployments remotely via the FlubuCore Web API.](https://flubucore.dotnetcore.xyz/WebApi/getting-started/)

* [Possibility to use FlubuCore tasks in any other .NET application.](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs)

* [FlubuCore interactive mode](https://flubucore.dotnetcore.xyz/build-script-runner-interactive/) which offers target tab completition, options tab completition, toogle targets/options, executed commands history. It is also possible to execute external commands and operable programs. For some of them FlubuCore offers tab completion with help displayed at the bottom of console out of the box(such as dotnet, git..)  

![FlubuCore interactive mode](https://raw.githubusercontent.com/dotnetcore/flubu.core/master/assets/FlubuCore_Interactive_mode_full.gif)

* Improved developer experience with FlubuCore custom analyzers.

![FlubuCore analyzers in action](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCoreCustomAnalyzerDemo.png)

## **Getting Started**
Using FlubuCore is straightforward and very simple :-) It is also fully and throughly documented.

The [Getting Started](https://flubucore.dotnetcore.xyz/getting-started/) chapter in [Documentation](https://flubucore.dotnetcore.xyz) will help you set up your first FlubuCore build in no time. 
You should also check [getting started blog.](https://www.huanlintalk.com/2020/04/getting-started-with-flubucore.html) It has some more details with some nice tips and tricks.

A comprehensive list of features that FlubuCore has to offer with descriptions can be found in the [Build Script Fundamentals](https://flubucore.dotnetcore.xyz/buildscript-fundamentals) chapter.

Once you have your build and deployment scripts defined, the following Wiki chapters will explain how to run them:

* For .NET Framework projects use [FlubuCore.Runner](https://flubucore.dotnetcore.xyz/getting-started#Installation.net)

* For .NET Core projects use [FlubuCore CLI global tool](https://flubucore.dotnetcore.xyz/getting-started#Installation-.net-core)

## **Examples**
Aside from the detailed Wiki FlubuCore comes with example projects that reflect real-life situations. The examples can be found in the separate [Examples repository](https://github.com/flubu-core/examples/).

These examples will help you to get quickly start with FlubuCore:
* [.NET Framework build example](https://github.com/flubu-core/examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs
) - Example covers versioning, building the project, running tests, packaging application for deployment.

* [.NET Core build example](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs
) - Example covers versioning, building the project, running tests, packaging application for deployment.

* [Deployment script example](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs
) - Example shows how to write simple deployment script. 


* [Open source library example](https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCoreOpenSource/Build/BuildScript.cs) - Example covers versioning, building the project, running tests and publishing nuget package. It also covers how to run build script on Appveyor and Travis CI.

## **Have a question?**

 [![Join the chat at https://gitter.im/FlubuCore/Lobby](https://badges.gitter.im/mbdavid/LiteDB.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## **Contributing**

Please see [CONTRIBUTING.md](https://github.com/dotnetcore/FlubuCore/blob/master/CONTRIBUTING.md).

## **Ways to Contribute**

* Spread the word about the project.
* If you like the project don't forget to give it a star so that the community get's bigger.
* Improve documentation.
* Report, fix a bug.
* Implement a new feature.
* Discuss potential ways to improve project.
* Improve existing implementation, performance, etc.

## Further Development
If you find FlubuCore useful (you feel it helps you on the daily basis) you can support further development by buying us a coffee (or become a backer or sponsor). Sometimes it's hard to stay awake till midnight implementing new features, coffee helps us with that. We would really appreciate your support. Money from sponsorship will also be used for the promotion of the project. If you are a backer or a sponsor you can also request for a new feature or ask for support. These issues will be handled with highest priority.

[![](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/NCOpMkt)

### Backers and Sponsors
[![OpenCollective](https://opencollective.com/flubucore/backers/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12502) 
[![OpenCollective](https://opencollective.com/flubucore/sponsors/badge.svg?style=for-the-badge)](https://opencollective.com/flubucore/order/12503)

## **Changelog and Roadmap**

Changes with description and examples can be found in [Changelog.](https://github.com/flubu-core/flubu.core/blob/master/CHANGELOG.md) 
 
You can see FlubuCore roadmap by exploring opened [Milestones.](https://github.com/flubu-core/flubu.core/milestones)

## Acknowledgements

* Special thanks to [@ironcev](https://github.com/ironcev) for greatly improving readme and for giving some valuable advices.
* Special thanks to [@alexinea](https://github.com/https://github.com/alexinea) for translating whole documentation to Chinese.
* Special thanks to [@huanlin](https://github.com/huanlin) for writing blogs about FlubuCore in Traditional Chinese and for translating them to English.

## .NET Foundation

This project is supported by the [.NET Foundation](http://www.dotnetfoundation.org).
