## **Build script**


Each build script should inherit from DefaulBuildScript class. Two abstact methods from DefaultBuildScript have to be implemented.

- ConfigureTargets: Here you can create new targets that will perform specific work.

- ConfigureBuildProperties: Here you can set various build properties which can be shared between multiple tasks and custom code.

Empty build script example

```C#
public class BuildScript : DefaultBuildScript
{
	protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
    }

    protected override void ConfigureTargets(ITaskContext session)
    {
    }
}
```

<a name="Targets"></a>

## **Targets**
-------

Targets are used to perform specific work in a specific order. A target can for example execute flubu built in tasks like a task for compiling the solution or it can execute some custom code. Target can also have dependencies on other targets.

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

Target's can also be defined with attribute on method.

```C#
[Target("targetName", "a", "b")]
[Target("targetName2", "c", "d")]
[Target("targetName3", "e", "f")]
public void Example(ITarget target, string source, string destination)
{
	target.AddTask(x => x.CopyFileTask(source, destination, true));
}
```

You can also pass values to parameter through console arguments, FlubuCore config file.

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

All Tasks also have following methods
  
- ``` .OnError((c, ex) => { c.LogInfo("Example");})) ``` - onError can perform some custom action when error occurs on single task

- ``` .Retry(5, 1000) ``` - Retry mechanism. You can apply specific condition when retry mechanism will retry task.

- ``` .Finally(c => { c.LogInfo("Example");})) ``` - Finally block acts just like finally in try catch

- ``` .DoNotFailOnError() ``` - script does not fail in case of exception. You can apply specific condition when task will not fail. 

- ``` .NoLog() ``` - Task doesn't log anything to console output.

- ``` .SetDescription() ``` - Overrides the default help description of the task.

- ``` .ForMember() ``` - pass through console argument to method or property. See [Pass console arguments, settings from json configuration file, environment variables with ForMember to tasks.](#Arguments-pass-through-to-tasks) for more details.

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

<a name="Custom-code"></a>

### **Custom code / tasks**

Following example executes some custom code. You can also use built in flubu tasks in custom code as shown in example.

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

You can also pass arguments to custom code:

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

When targetC is executed target’s will be executed in the following order: TargetB, TargetA, TargetC

```C#
var targetA = context.CreateTarget("TargetA");
var targetB = context.CreateTarget("TargetB");
var targetC = context.CreateTarget("TargetC").DependsOn(targetB, targetA);      
```

<a name="Reuse-set-of-tasks"></a>

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

### **Async execution of tasks, customCode and dependencies**

<ul>
<li>
Tasks can be executed asynchrounously with AddTaskAsync or AddCoreTaskAsync method.

</li>
<li>
Custom code can be executed asynchrounosly with DoAsync method.

</li>
<li>
Dependencies can be executed asynchrounosly with DependsOnAsync method.

</li>
</ul>
Following target executes 3 tasks asynchorunusly.

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

The code above will first execute 2 nunit tasks asynchronously and wait for both tasks to finish. Then it will execute SomeCustomMethod synchrounosly. After it is finished code from SomeCustomAsyncMethod2 and SomeCustomAsyncMethod3 will be executed asynchronously.

<a name="Other-features"></a>

### **Other features**

-   SetAsDefault method: When applied to target that target is runned by default if no target is specified when running the script with runner.
-   SetAsHidden method: When applied to target that target is not shown in help and it can only be run as other target dependecie.
- GetEnviromentVariable method: Get's the enviroment variable.
- Must method: Condition in must will have to be meet otherwise target execution will fail before any task get executed.
- Log:`.LogInfo("Some Text2", ConsoleColor.Blue);`

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
protected override void ConfigureTargets(ITaskContext session)
{
    var runExternalProgramExample = session.CreateTarget("systemctl.example")
        AddTask(x => x.RunProgramTask(@"systemctl")             
            .WithArguments("start")
            .WithArguments("nginx.service"));
}
```

<a name="Build-properties"></a>

### **Build properties**

You can define various build properties in ConfigureBuildProperties method to share them in different tasks and custom code.

Following example show how to share nunit console path across various nunit targets/tasks.

```C#
protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
{
	context.Properties.Set(BuildProps.NUnitConsolePath, @"packages\NUnit.ConsoleRunner.3.6.0\tools\nunit3-console.exe");
}

protected override void ConfigureTargets(ITaskContext session)
{
	session.CreateTarget("unit.tests1")
        .SetDescription("Runs unit tests")
        .AddTask(x => x.NUnitTaskForNunitV3("FlubuExample.Tests"));
		
    session.CreateTarget("unit.tests1")
         AddTask(x => x.NUnitTaskForNunitV3("FlubuExample.Tests2"));
}
```

If nunit console path would not be set in build properties you would have to set it in each task separately.

like so:
```C#
protected override void ConfigureTargets(ITaskContext session)
{
    session.CreateTarget("unit.tests1")
        .SetDescription("Runs unit tests")
        .AddTask(x => x.NUnitTaskForNunitV3("FlubuExample.Tests")
            .NunitConsolePath(@"packages\NUnit.ConsoleRunner.3.6.0\tools\nunit3-console.exe"));
			
    session.CreateTarget("unit.tests1")
		.AddTask(x => x.NUnitTaskForNunitV3("FlubuExample.Tests2").
			NunitConsolePath(@"packages\NUnit.ConsoleRunner.3.6.0\tools\nunit3-console.exe"));
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

   `{
      "sn" : "true",
      "SomeOtherKey" : "SomeOtherValue"
    }` 

* It's typical to have different configuration settings for different environments, for example, development, testing, and production. Just create different files FlubuSettings.{Enviroment}.Json and [set enviroment variable](https://andrewlock.net/how-to-set-the-hosting-environment-in-asp-net-core/) 'ASPNETCORE_ENVIRONMENT' on desired machine 
* You can also create json configuration file by machine name FlubuSettings.{MachineName}.Json. If MachineName in file matches the machine name Flubu will automatically read settings from that file.

<a name="enviroment-variable"></a>

### **Passing enviroment variable to build script property**

You can also set script arguments through environment variables. environment variables must have prefix flubu_

For above example you would add environment variable from windows command line with the following command:

`set flubu_sn=true`

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

* First parameter is the method or property argument will be passed through. values set in method parameters are default values if argument is not specified when running the build script.
* Second parameter is the argument key.
* Third optional parameter is help that it will be displayed in detailed target help. If parameter is not set then default generated help will be displayed.

 `Dotnet flubu compile -solution=someothersolution.sln`

<a name="Referencing-other-assemblies-in-build-script"></a>

## **Referencing external assemblies in build script**

FlubuCore loads all assemblies references and nuget packages automatically from build script csproj. Csproj must be at on of the location specified [here](https://github.com/flubu-core/flubu.core/blob/master/FlubuCore/Scripting/Analysis/ProjectFileAnalyzer.cs) If not assembly and nuget references will not be loaded automatically when executing script.

Note: You can also disable referencing assemblies and nuget packages from build script by adding attribute to build script.

```C#
[DisableLoadScriptReferencesAutomatically]
public class BuildScript : DefaultBuildScript
{
}
```

Alternatively when you are running scripts without csproj(for example deploy scripts) external references can be added  with directives in three ways:

<a name="By-assembly-relative-or-full-path"></a>

### **By assembly relative or full path**

On the build script class you have to add attribute:

```C#
[Assembly(@".\packages\Newtonsoft.Json.9.0.1\lib\net45\Newtonsoft.Json.dll")]
public class BuildScript : DefaultBuildScript
{
    public void ReferencedAssemlby(ITaskContext context)
    {
       JsonConvert.SerializeObject("Example");
    }
}
```
FlubuCore can also load all assemblies from specified directory and optionaly from it's subdirectories

```C#
[AssemblyFromDirectory(@".\Packages", true)]
public class BuildScript : DefaultBuildScript
{
}
```

<a name="Referencing-nuget-packages"></a>

### **Referencing nuget packages**

Flubu supports referencing nuget packages. .net core sdk or msbuild must be installed if u want to reference nuget packages otherwise they will not get restored.

You have to add NugetPackage attribute on the script class:

```C#
[NugetPackage("Newtonsoftjson", "11.0.2")]
public class BuildScript : DefaultBuildScript
{
    public void ReferencedNugetPackage(ITaskContext context)
    {
       JsonConvert.SerializeObject("Example");
    }
}
```

<a name="Load-assembly-by-assembly-full-name"></a>

### **Load assembly by assembly full name**

System assemblies can be loaded by fully qualifed assemlby name.

You have to add Reference attribute on the script class:

```C#
[Reference("System.Xml.XmlDocument, System.Xml, Version=4.0.0.0, Culture=neutral, publicKeyToken=b77a5c561934e089")]
public class BuildScript : DefaultBuildScript
{
    public void ReferencedAssemlby(ITaskContext context)
    {
		XmlDocument xml = new XmlDocument();
    }
}
```

One way to get fully qualifed assembly name:

    var fullQualifedAssemblyName = typeof(XmlDocument).Assembly.FullName;

<a name="Load-all-assemblies-from-directory"></a>

### **Load all assemblies from directory**
Even if you are not using your script together with csproj flubu can load all external assemblies for you automatically from directory (assemblies in subdirectories are also loaded ). 

By default flubu loads all assemblies from directory FlubuLib. Just create the directory at the flubu runner location and put assemblies in that directory. You can specify directory in flubu runner from where to load assemblyes also:

`flubu.exe -ass=somedirectory`

`dotnet flubu -ass=somedirectory`
alternatively you can put ass key into flubusettings.json file:

    {
      "ass" : "someDirectory",
      "SomeOtherKey" : "SomeOtherValue"
    }` 

<a name="Adding-other-cs-files-to-build-script"></a>

## **Adding other .cs files to script**

On the build script class you have to add attribute:

```C#
[Include(@".\BuildHelper.cs")]
public class BuildScript : DefaultBuildScript
{
    public void Example(ITaskContext context)
    {
        BuildHelper.SomeMethod();
    }
}    
```

FlubuCore can also load all .cs files to script from specified directory and optionaly from it's subfolders.

```C#
[IncludeFromDirectory(@".\Helpers", true)]
public class BuildScript : DefaultBuildScript
{
}
```

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

Partial and base classes are loaded automatically if they are located in the same directory as buildscript. Otherwise they have to be added with Include attribute. 