## **Build script**


Each build script should inherit from DefaulBuildScript class. Two abstact methods from DefaultBuildScript have to be implemented.

- ConfigureTargets: Here you can create new targets that will perform specific work.

Empty build script example

```C#
public class BuildScript : DefaultBuildScript
{

    protected override void ConfigureTargets(ITaskContext session)
    {
    }
}
```

<a name="Targets"></a>

## **Targets**
-------

Targets are used to perform specific work in a specific order. A target can for example execute flubu built in tasks like a task for compiling the solution or it can execute some custom csharp code. Target can also have dependencies on other targets.

### **Create a new Target**

Following code will create a new target that will execute a built in task.

```C#
protected override void ConfigureTargets(ITaskContext context)
{
	context.CreateTarget("Compile")
		.SetDescription("Compiles the solution")
        .AddTask(x => x.CompileSolutionTask());
}
```

After target is defined. Target can be executed from console `Flubu compile`

Target's can also be defined with attributes on method.

```C#
[Target("targetName", "a", "b")]
[Target("targetName2", "c", "d")]
[Target("targetName3", "e", "f")]
public void Example(ITarget target, string source, string destination)
{
	target.AddTask(x => x.CopyFileTask(source, destination, true));
}
```

You can also pass values to parameter through console arguments or FlubuCore config file.

`Flubu targetName2 -destination=SomeOtherDestination`

<a name="Tasks"></a>

### **Tasks**

Tasks are divided in tasks and core tasks. tasks can be executed in .net and .net core projects. Core tasks can only be executed in .net core projects.

Following example executes 2 core tasks in a target. Order of execution is the same as specified in code.

```C#  
context.CreateTarget("Build")
    .AddCoreTask(x => x.Restore())
    .AddCoreTask(x => x.Build());
```

All Tasks have following methods:
  
- ``` .OnError((c, ex) => { c.LogInfo("Example");})) ``` - onError can perform some custom action when error occurs on specific task.

- ``` .Retry(5, 1000) ``` - Retry mechanism. You can apply specific condition when retry mechanism will retry task.

- ``` .Finally(c => { c.LogInfo("Example");})) ``` - Finally block acts just like finally in try catch.

- ``` .DoNotFailOnError() ``` - script does not fail in case of exception. You can apply specific condition when task will not fail. 

- ``` .NoLog() ``` - Task doesn't log anything to console output.

- ``` .SetDescription() ``` - Overrides the default help description of the task.

- ``` .ForMember() ``` - pass through console argument to method or property. See [Pass console arguments, settings from json configuration file, environment variables with ForMember to tasks](#Arguments-pass-through-to-tasks) for more details.

- conditonal task execution with when cluase on single task (see bellow for group of tasks)

```c#
context.CreateTarget("Example")
	.AddTask(x => x.CompileSolutionTask())
    .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec"))
    .When(c => c.BuildSystems().Jenkins().IsRunningOnJenkins);
```

- set task parameters only when specified condition is meet.

```c#
 var compile = context
	.CreateTarget("compile")
    .SetDescription("Compiles the VS solution")
    .AddCoreTask(x => x.Build().Configuration("Release")
	.When(
		() =>
		{
	    	return context.BuildSystems().IsLocalBuild;
	    }, 
		task => { task.Configuration("Debug"); }));
```

- ```.Interactive()``` - Interactively pass argument from console to specified task method / parameter.

#### **Task attributes (Versioning)**
It is possible to execute some tasks with attribute on property

Flubu will inject return value of the task to the property. This is especially usefull for all versioning tasks, basically all tasks that return a value. See `FlubuCore.Tasks.Attributes` namespace for all available attributes.

``` C#     
[FetchBuildVersionFromFile]
public BuildVersion BuildVersion { get;  }
```   

``` C#     
[GitVersion]
public GitVersion GitVersion { get;  }
```

This allows you to access version information in ConfigureTarget which is not possible if versioning task is executed for example as target dependency

``` C# 
protected override void ConfigureTargets(ITaskContext context)
{
        context.CreateTarget("Build")
            .AddCoreTask(x => x.Build().Version(BuildVersion.Version.ToString()));
}
```  

<a name="Custom-code"></a>

### **Write custom c# code (custom tasks)**

Following example executes custom code. You can also use built in flubu tasks in custom code as shown in example.

```C#
protected override void ConfigureTargets(ITaskContext context)
{
	context.CreateTarget("Example")
       .Do(CustomCodeExample);
}

private static void CustomCodeExample(ITaskContext context)
{
    //// You can put any c# code here and use any .net libraries.
    Console.WriteLine("Dummy custom code");
    context.Tasks().NUnitTaskForNunitV3("project name").Execute(context);
}
```

You can also add parameters to methods:

```C#
protected override void ConfigureTargets(ITaskContext context)
{
	context.CreateTarget("Example")
		.Do(CustomCodeExample, "some value", 1);
}

private static void CustomCodeExample(ITaskContext context, string arg1, int arg2)
{
	Console.WriteLine("Dummy custom code");
    context.Tasks().NUnitTaskForNunitV3("project name").Execute(context);
}
```

<a name="Target-dependencies"></a>

### **Target dependencies**

Target can have dependencies on other targets. All dependenies will be executed before target in the specified order.

When targetC is executed target’s will execute in the following order: TargetB, TargetA, TargetC

```C#
var targetA = context.CreateTarget("TargetA");
var targetB = context.CreateTarget("TargetB");
var targetC = context.CreateTarget("TargetC").DependsOn(targetB, targetA);      
```

It is also possible to reverse dependency

```C#
var targetC = context.CreateTarget("TargetC").DependenceOf(targetA);      
```

<a name="Reuse-set-of-tasks"></a>

### **Add target to target**

Target can be executed within other target with AddTarget. Target is executed in the order it was added

Example:
``` C#
    protected override void ConfigureTargets(ITaskContext context)
    {
       var exampleB = context.CreateTarget("TargetB")
            .Do(Something);

       context.CreateTarget("TargetA")
           .AddCoreTask(x => x.Build())
           .AddTarget(exampleB)
           .Do(JustAnExample);
    }

    public void JustAnExample(ITaskContext context)
    {
        ...
    }
```
following execution order is taken when  TargetA is executed

1. Build task
2. TargetB target
3. JustAnExample method


### **Reuse set of tasks in different targets**

Following example shows how to reuse set of tasks in different targets:

```C#
protected override void ConfigureTargets(ITaskContext session)
{
	session.CreateTarget("deploy.local").AddTasks(Deploy, "c:\\ExamplaApp").SetAsDefault();
                
    session.CreateTarget("deploy.test").AddTasks(Deploy, "d:\\ExamplaApp");

    session.CreateTarget("deploy.prod").AddTasks(Deploy, "e:\\ExamplaApp");
}

private void Deploy(ITarget target, string deployPath)
{
    target
        .AddTask(x => x.IisTasks().CreateAppPoolTask("Example app pool").Mode(CreateApplicationPoolMode.DoNothingIfExists))
        .AddTask(x => x.IisTasks().ControlAppPoolTask("Example app pool", ControlApplicationPoolAction.Stop).DoNotFailOnError())
        .Do(UnzipPackage)
        .AddTask(x => x.CopyDirectoryStructureTask(@"Packages\ExampleApp", @"C:\ExampleApp", true).Retry(20, 5000))
        .Do(CreateWebSite)
}    
```

### **Add tasks to target with a foreach loop**

Following example shows how to add multiple tasks to target with a foreach loop

```c#
  protected override void ConfigureTargets(ITaskContext context)
  {
         var solution = context.GetVsSolution();

         context.CreateTarget("Pack")
                .ForEach(solution.Projects, (item, target) =>
                {
                    target.AddCoreTask(x => x.Pack().Project(item.ProjectName))
                          .Do(JustAnExample, item);
                });
  }

  private void JustAnExample(ITaskContext context, VSProjectInfo vsProjectInfo)
  {
        //// Do something.
  }
```

Example will execute Pack task for each project in solution.

<a name="Group-task"></a>

### **Group tasks and apply When, OnError, Finally on them**

-  Conditonal task execution with When clause on group of tasks.

```C#
protected override void ConfigureTargets(ITaskContext context)
{
	context.CreateTarget("Example")
        .AddCoreTask(x => x.Build())
        .Group(
               target =>
               {
                    target.AddCoreTask(x => x.Pack());
                    target.AddCoreTask(x => x.NugetPush("pathToPackage"));
               },
               when: c => !c.BuildSystems().Jenkins().IsRunningOnJenkins);
}
```

- Finally on group of tasks: onFinally acts just like finally in try/catch.

```C#
context.CreateTarget("Example")
		.AddCoreTask(x => x.Build())
         .Group(
              target =>
              {
				 target.AddCoreTask(x => x.Pack());
                 target.AddCoreTask(x => x.NugetPush("pathToPackage"));
              },
              onFinally: c =>
              {
				 c.Tasks().DeleteFilesTask("pathToNupkg", "*.*", true).Execute(c);
              });
```

- OnError on group of tasks:  You can perform some custom action when error occures in any of tasks that are in group.

```C#
context.CreateTarget("Example")
    .AddCoreTask(x => x.Build())
    .Group(
        target =>
        {
			target.AddCoreTask(x => x.Pack());
            target.AddCoreTask(x => x.NugetPush("pathToPackage"));
        },
        onError: (c, error) =>
        {
           //// some custom action when error occures in any of the task in group.
        });
```

<a name="Async-execution"></a>

### **Asynchronus or parallel execution of tasks, customCode and dependencies**

<ul>
<li>
Tasks can be executed asynchrounously or in parallel with AddTaskAsync or AddCoreTaskAsync method.

</li>
<li>
Custom code can be executed asynchrounosly with DoAsync method.

</li>
<li>
Dependencies can be executed asynchrounosly with DependsOnAsync method.

</li>
</ul>
Following target executes 3 tasks in parallel.

```C#
session.CreateTarget("run.tests")
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName3"));
```

Async and sync methods can also be mixed

```C#
session.CreateTarget("async.example")
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .Do(SomeCustomMethod)
    .DoAsync(SomeCustomAsyncMethod2)
    .DoAsync(SomeCustomAsyncMethod3);
```

The code above will first execute 2 nunit tasks asynchronously and wait for both tasks to finish. Then it will execute SomeCustomMethod synchrounosly. After it is finished code from SomeCustomAsyncMethod2 and SomeCustomAsyncMethod3 will be executed in parallel.

#### sequential logging in asynchronus executed tasks and targets 

Usually logs are not readable when executing more than 1 task asynchronously or in parallel. That's why FlubuCore offers sequential logging in asynchronus tasks. You can enable them with  ` .SequentialLogging(true)` on target. It has to be placed before asynchronus tasks/target dependencies otherwise logs will not be sequential.
```c#
context.CreateTarget("Test")
        .SetAsDefault()
        .SequentialLogging(true)
        .AddCoreTaskAsync(x => x.Pack())
        .AddCoreTaskAsync(x => x.Pack())
        .DependsOnAsync(test2, test3);
```
Target executed in parallel with FlubuCore runner have sequential logging on by default.

`flubu target1 target2 --parallel`

<a name="Other-features"></a>

### **Other features**

#### Target features
- SetAsDefault method: When applied to target that target is runned by default if no target is specified when running the script with runner.
- SetAsHidden method: When applied to target that target is not shown in help and it can only be run as other target dependency.
- Must method: Condition in must will have to be meet otherwise target execution will fail before any task get executed.
- Requires method: Parameter specified in required method must not be null otherwise target execution will fail before any task get executed.

#### Context features

- Log:`context.LogInfo("Some Text2", ConsoleColor.Blue);`
- GetVsSolution: Get's solution and it's projects information(such as full project path, target framework, runtimeidentifier..) `context.GetVsSolution();`
- GetFiles: Get Files from specified directory with option to filter files with glob pattern `context.GetFiles(OutputDirectory, "*.nupkg");`
- GetDirectories: Get Directories from specified directory with option to filter files with glob pattern `context.GetFiles(OutputDirectory, "*.nupkg");`
- GetEnviromentVariable method: Get's the enviroment variable by name  `context.GetEnvironmentVariable("someVariable");`

<a name="Run-any-program"></a>

#### Run any program or command in build script with RunProgramTask

```C#
protected override void ConfigureTargets(ITaskContext session)
{
	var runExternalProgramExample = session.CreateTarget("run.libz")
        .AddTask(x => x.RunProgramTask(@"packages\LibZ.Tool\1.2.0\tools\libz.exe")
            .WorkingFolder(@".\src")
            .WithArguments("add")
            .WithArguments("--libz", "Assemblies.libz"));
 }
```

Linux Example:

```C#
protected override void ConfigureTargets(ITaskContext context)
{
    var runExternalProgramExample = context.CreateTarget("systemctl.example")
        AddTask(x => x.RunProgramTask(@"systemctl")             
            .WithArguments("start")
            .WithArguments("nginx.service"));
}
```

<a name="Build-properties"></a>

### **Build properties**

You can define various build properties with Attributes on properties or in ConfigureBuildProperties method (old way) to share them in different tasks and custom code.

Following example shows how to share solution file name and configuration across various targets/tasks.

```C# 
    [SolutionFileName]
    public string SolutionFileName { get; set; } = "FlubuExample.sln";
   
    [BuildConfiguration] 
    public string BuildConfiguration { get; set; } = "Release";

protected override void ConfigureTargets(ITaskContext context)
{
	   context.CreateTarget("build")
            .AddCoreTask(x => x.Build());

        context.CreateTarget("pack")
            .AddCoreTask(x => x.Pack());
}
```

Alternative:
```C# 
    [BuildProperty(BuildProps.BuildConfiguration)]
    public string BuildConfiguration { get; set; } = "Release";
```
If Solution file name and path would not be set through build property attributes you would have to set it in each task separately.

like so:
```C#
protected override void ConfigureTargets(ITaskContext context)
{
     context.CreateTarget("build")
        .AddCoreTask(x => x.Build()
            .Project("FlubuExample.sln")
            .Configuration("Release"));

    context.CreateTarget("pack")
        .AddCoreTask(x => x.Pack()
            .Project("FlubuExample.sln")
            .Configuration("Release"));
}
```

<a name="Predefined-build-properties"></a>

#### Predefined build properties

 Some build properties are already defined. You can access them through interface:
  
` context.Properties.Get(PredefinedBuildProperties.OsPlatform);`

Available predefined build properties:

* OsPlatform
* PathToDotnetExecutable
* UserProfileFolder
* OutputDir 
* ProductRootDir

All of them can be overriden.

<a name="Script-arguments"></a>

## **Pass command line arguments, settings from json configuration file or environment variables to your build script properties.**

 You can pass command line arguments, settings from json configuration file or environment variables to your build script properties by adding FromArg attribute to property. 

```C#
public class SimpleScript : DefaultBuildScript
{
    [FromArg("sn", "If true app is deployed on second node. Otherwise not.")]
    public bool deployOnSecondNode { get; set; }        

    protected override void ConfigureTargets(ITaskContext context)
    {
        context.CreateTarget("Deploy.Exapmle")
            .AddTask(x => x.FlubuWebApiTasks().GetTokenTask("user", "pass").SetWebApiBaseUrl("noade1Url"))
            .AddTask(x => x.FlubuWebApiTasks().UploadPackageTask("packageDir", "*.zip"))
            .AddTask(x => x.FlubuWebApiTasks().ExecuteScriptTask("Deploy", "DeployScript.cs"))
            .Group(target =>
            {
                target.AddTask(x => x.FlubuWebApiTasks().GetTokenTask("user", "pass").SetWebApiBaseUrl("noade2Url"))
                      .AddTask(x => x.FlubuWebApiTasks().UploadPackageTask("packageDir", "*.zip"))
                      .AddTask(x => x.FlubuWebApiTasks().ExecuteScriptTask("Deploy", "DeployScript.cs"));
            },
            when: c => deployOnSecondNode);         
    }
}
```

First parameter in FromArg attribute is the argument key. Second is the help description of the property shown in flubu runner. You actually don't need to put attribute on property. If u dont then the key is the same as property name and help is not shown for property in build script runner.

Property types that are supported: string, boolean, int, long, decimal, double, DateTime.

<a name="Command-line-argument"></a>

### **Passing command line argument to build script property.**

 `Dotnet flubu Deploy.Example -sn=true`

<a name="json-configuration-file"></a>

### **Passing setting from json configuration file to build script property**

 * Create file FlubuSettings.json where Flubu runner is located.
 * Add argument key and value to file in json format.
 * For above example json file would look like this:
	```json
	 {
		"sn" : true,
		"SomeOtherKey" : "SomeOtherValue"
	 } 	
	```
* It's typical to have different configuration settings for different environments for example development, testing, and production. Just create different json files `FlubuSettings.{Environment}.Json` and [set enviroment variable](https://andrewlock.net/how-to-set-the-hosting-environment-in-asp-net-core/) 'ASPNETCORE_ENVIRONMENT' on desired machine 
* You can also create json configuration file by machine name `FlubuSettings.{MachineName}.Json`. If MachineName in file matches the machine name Flubu will automatically read settings from that file.

<a name="enviroment-variable"></a>

### **Passing enviroment variable to build script property**

You can also set script arguments through environment variables. environment variables must have prefix `flubu_`

For above example you would add environment variable from windows command line with the following command: `set flubu_sn=true`

<a name="Arguments-pass-through-to-tasks"></a>

## **Pass console arguments, settings from json configuration file, environment variables with ForMember to tasks.**

There is an alternative more sophisticated way to pass console arguments, settings and environment variables to tasks

```C#
protected override void ConfigureTargets(ITaskContext context)
{
   context.CreateTarget("compile")
       .AddTask(x => x.CompileSolutionTask()
           .ForMember(y => y.SolutionFileName("someSolution.sln"), "solution", "The solution to build."));
}
```

* First parameter is the method or property argument that will be passed through. values set in method parameters are default values if argument is not specified when running the build script.
* Second parameter is the argument key.
* Third optional parameter is help that will be displayed in detailed target help. If parameter is not set then default generated help will be displayed.

 `Dotnet flubu compile -solution=someothersolution.sln`

<a name="Referencing-other-assemblies-in-build-script"></a>



<a name="Build-system-providers"></a>

## **Build system providers** 

You can acces various build, commit... information for various build systems (such as Jenkins, TeamCity, AppVeyor, Travis...) 

```C#
protected override void ConfigureTargets(ITaskContext context)
{
    bool isLocalBuild = context.BuildSystems().IsLocalBuild;
    var gitCommitId = context.BuildSystems().Jenkins().GitCommitId;
}
```

<a name="Before-After"></a>

## **Build events**

- OnBuildFailed event:

```c#
public class BuildScript : DefaultBuildScript
{
    protected override void OnBuildFailed(ITaskSession session, Exception ex)
    {
    } 
}
```
 
- before and after target execution events:

```c#
protected override void BeforeTargetExecution(ITaskContext context)
{
}

protected override void AfterTargetExecution(ITaskContext context)
{
}
```    

- before and after build execution events:

```c#
protected override void BeforeBuildExecution(ITaskContext context)
{
}

protected override void AfterBuildExecution(ITaskSession session)
{
}
```
<a name="partial-class"></a>

## **Partial and base class in script**

Partial and base classes are loaded automatically if they are located in the same directory as buildscript. Otherwise they have to be added with [Include attribute](../referencing-external-assemblies#adding-other-cs-files-to-script). 