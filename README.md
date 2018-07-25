# FlubuCore

[![AppVeyor](https://img.shields.io/appveyor/ci/flubu-core/flubu.core.svg?label=Windows%20build)](https://ci.appveyor.com/project/flubu-core/flubu.core)
[![Travis](https://img.shields.io/travis/USER/REPO.svg?label=Linux%20build)](https://TODO)
[![NuGet](https://img.shields.io/nuget/v/FlubuCore.svg)](https://www.nuget.org/packages/FlubuCore/)
[![Gitter](https://img.shields.io/gitter/room/FlubuCore/Lobby.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![License](https://img.shields.io/github/license/flubu-core/flubu.core.svg)](https://github.com/flubu-core/flubu.core/blob/master/LICENSE)

"FlubuCore - Fluent Builder Core" is a cross platform build and deployment automation system. You can define your build and deployment scripts in C# using an intuitive fluent interface. This gives you code completion, IntelliSense, debugging, and native access to the whole .NET ecosystem inside of your scripts.

![FlubuCore in action](https://raw.githubusercontent.com/ironcev/flubu.core/master/demo.gif)

## Features and Advantages

* Intuitive an easy to learn. C#, fluent interface, and IntelliSense make even most complex script creation a breeze.

    ```
    context.CreateTarget("Example")
      .AddTask(x => x.CompileSolutionTask())
      .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec"))
          .When(c => c.BuildSystems().Jenkins().IsRunningOnJenkins);
    ```
          
* [Large number of often used built-in tasks](https://github.com/flubu-core/flubu.core/wiki/4-Tasks) like e.g. running tests, managing IIS, creating deployment packages, publishing NuGet packages, executing PowerShell scripts and many more.

    ```
    target
        .AddTask(x => x.CompileSolutionTask())
        .AddTask(x => x.CopyFileTask(source, destination, true))
        .AddTask(x => x.IisTasks()
                        .CreateAppPoolTask("Example app pool")
                        .Mode(CreateApplicationPoolMode.DoNothingIfExists));
    ```

* [Execute your own custom C# code.](https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Custom-code)

    ```
    context.CreateTarget("MyCustomBuildTarget")
         .Do(MyCustomMethod);
    ```

* [Reference any .NET library, NuGet package or C# source code in your scripts.](https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Referencing-other-assemblies-in-build-script)

    ```
    //#nuget Newtonsoftjson, 11.0.2
    public class BuildScript : DefaultBuildScript
    {
       public void NuGetPackageReferencingExample()
        {
            JsonConvert.SerializeObject("Example");
        }
    }
    ```

* [Easily run any external program or console command in your script.](https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Run-any-program)

    ```
    session.CreateTarget("Run.Libz")
        .AddTask(x => x.RunProgramTask(@"packages\LibZ.Tool\1.2.0\tools\libz.exe")
            .WorkingFolder(@".\src")
            .WithArguments("add")
            .WithArguments("--libz", "Assemblies.libz"));
    ```

* [Extending FlubuCore fluent interface by writing your own FlubuCore tasks.](https://github.com/flubu-core/flubu.core/wiki/5-How-to-write-and-use-FlubuCore-task-plugins)

    ```
    public class ExampleFlubuPluginTask : TaskBase<int, ExampleFlubuPluginTask>
    {
        protected override int DoExecute(ITaskContextInternal context)
        {
            // Write your task logic here.
            return 0;
        }
    }
    ```

* [Asynchronous execution of tasks, target dependencies and custom code.](https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Async-execution)

    ```
    session.CreateTarget("Run.Tests")
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
        .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName3"));
    ```

* [Full .NET Core support including the global CLI tool](https://github.com/flubu-core/flubu.core/wiki/1-Getting-started#getting-started-net-core)

    ```
    dotnet tool install --global FlubuCore.GlobalTool
    flubu compile
    ```

* [Possibility to test and debug your build scripts.](https://github.com/flubu-core/flubu.core/wiki/6-Writing-build-script-tests,-debuging-and-running-flubu-tasks-in-other--.net-applications)

    ```
    context.WaitForDebugger();
    ```

* [Easily automate deployments remotely via the FlubuCore Web API.](https://github.com/flubu-core/flubu.core/wiki/7-Web-Api:-Getting-started)

* [Possibility to use FlubuCore tasks in any other .NET application.](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs)

See [**getting started**](https://github.com/flubu-core/flubu.core/wiki/1-Getting-started) on wiki to get you started with FlubuCore. We know you don't belive it but it really is very simple :)

List of features that FlubuCore has to offer with description can be found at [**build script fundamentals**](https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals). Alternatively You can take a look at examples below.

* [For .NET projects use **FlubuCore.Runner**](https://github.com/flubu-core/flubu.core/wiki/1-Getting-started#Installation.net)
* [For .NET Core projects use CLI tool **dotnet-flubu**](https://github.com/flubu-core/flubu.core/wiki/1-Getting-started#Installation-.net-core)
* [Most elegant way for .NET Core projects is to use global tool](https://github.com/flubu-core/flubu.core/wiki/1-Getting-started#Run-build-script-core-with-global-tool) - ```dotnet tool install --global FlubuCore.GlobalTool```

## Examples

* [**.net example**](https://github.com/flubu-core/examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs
)

* [**.net core example**](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs
)

* [**.deploy script example**](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs
)

## Have a question?

 [![Join the chat at https://gitter.im/FlubuCore/Lobby](https://badges.gitter.im/mbdavid/LiteDB.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## Contribution Guidelines
* If u find a bug please report it :) 
* If u have any improvement or feature proposal's we would be glad to hear from you. Add new issue as proposal and we will discuss it with you.
* If u want to fix a bug yourself, improve or add new feature to flubu.. Fork, Pull request but first add new issue so we discuss it. You can also search the issues by label Help wanted, Good first issue or any other if u know how to fix/implement it. 


## Release Notes

Release notes can be found [here](https://github.com/flubu-core/flubu.core/blob/master/FlubuCore.ProjectVersion.txt).