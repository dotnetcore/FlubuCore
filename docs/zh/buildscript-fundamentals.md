## **构建脚本**

每个构建脚本（build script）都继承自 DefaulBuildScript 类，因此必须实现 DefaulBuildScript 中的两个抽象方法（abstract method）。

- ConfigureTargets：用于创建将执行特定工作的新目标（targets）。

空构建脚本示例：

```C#
public class BuildScript : DefaultBuildScript
{
    protected override void ConfigureTargets(ITaskContext session)
    {
    }
}
```

<a name="Targets"></a>

## **目标**

---

目标（target）用于按特定顺序执行特定工作。目标可执行诸如 FlubuCore 内置任务（如编译解决方案的任务）和一些自定义 C# 代码。目标也可以依赖于其他目标（other targets）。

### **创建新目标**

下例将创建在任务中执行一次构建的新目标。

```C#
protected override void ConfigureTargets(ITaskContext context)
{
	context.CreateTarget("Compile")
		.SetDescription("Compiles the solution")
        .AddTask(x => x.CompileSolutionTask());
}
```

目标也可以通过方法上的特性来进行定义。

```C#
[Target("targetName", "a", "b")]
[Target("targetName2", "c", "d")]
[Target("targetName3", "e", "f")]
public void Example(ITarget target, string source, string destination)
{
	target.AddTask(x => x.CopyFileTask(source, destination, true));
}
```

你还可以通过控制台参数（console arguments）或 FlubuCore 配置文件向参数（parameter）传递值。

`Flubu targetName2 -destination=SomeOtherDestination`

<a name="Tasks"></a>

### **任务**

任务分为两种类型：（一般）任务（task）和 Core 任务。（一般）任务可以在 .NET 和 .NET Core 项目中执行，而 Core 任务只能在 .NET Core 项目中执行。

下例代码，在目标中执行了两个 Core 任务，执行顺序与代码中指定的顺序一致。

```C#
context.CreateTarget("Build")
    .AddCoreTask(x => x.Restore())
    .AddCoreTask(x => x.Build());
```

所有任务都有以下方法：

- `.OnError((c, ex) => { c.LogInfo("Example");}))` - onError 可在指定任务发生错误时执行一些自定义操作；

- `.Retry(5, 1000)` - 重试机制（Retry Mechanism）。可在该机制重试任务期间应用特定条件（specific condition）；

- `.Finally(c => { c.LogInfo("Example");}))` - Finally 就像 try-cache-finally 里的 finally 块；

- `.DoNotFailOnError()` - 脚本不会因发生异常而失败，你可为任务不失败时应用特定条件；

- `.NoLog()` - 任务日志将不会输出到控制台；
-
- `.SetDescription()` - 覆盖（overrides）任务的默认描述；

- `.ForMember()` - 将控制台参数传递给方法或属性，相关详细信息请查阅[向任务传递控制台参数、JSON 配置文件的设置，以及基于 ForMember 的环境变量](#Arguments-pass-through-to-tasks);

- 在单个任务上使用 When 从句（when cluase），使任务有条件地执行（参阅下面的任务组）：

```c#
context.CreateTarget("Example")
	.AddTask(x => x.CompileSolutionTask())
    .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec"))
    .When(c => c.BuildSystems().Jenkins().IsRunningOnJenkins);
```

- 当且仅当满足指定条件时设置任务参数：

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

- `.Interactive()` - 交互地将参数从控制台传递给任务的方法或参数。

#### **任务特性（版本控制）**

可以通过属性上的特定（Attribute）来执行某些任务。

Flubu 将会把任务的返回值注入到属性中。这对所有版本控制任务、尤其是有返回值的任务特别有用。有关所有可用的特性，请参阅 `FlubuCore.Tasks.Attributes` 命名空间。

``` C#     
[FetchBuildVersionFromFile]
public BuildVersion BuildVersion { get;  }
```   

``` C#     
[GitVersion]
public GitVersion GitVersion { get;  }
```

你可以通过 ConfigureTarget 获取版本信息，而这些在版本控制任务已执行后是不可获得的（比如目标依赖项等）。

``` C# 
protected override void ConfigureTargets(ITaskContext context)
{
        context.CreateTarget("Build")
            .AddCoreTask(x => x.Build().Version(BuildVersion.Version.ToString()));
}
```  

<a name="Custom-code"></a>

### **自定义 C# 代码/任务**

下例将执行一些自定义代码。你可以在自定义代码中使用 FlubuCore 的内置任务：

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

你也可以使用带参方法：

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

### **目标依赖**

目标可依赖于其他目标。所有依赖项将按指定顺序在目标执行前执行。

当 targetC 执行时，目标的执行顺序将是：TargetB、TargetA 和 TargetC。

```C#
var targetA = context.CreateTarget("TargetA");
var targetB = context.CreateTarget("TargetB");
var targetC = context.CreateTarget("TargetC").DependsOn(targetB, targetA);
```

也可以反转依赖关系：

```C#
var targetC = context.CreateTarget("TargetC").DependenceOf(targetA);      
```


<a name="Reuse-set-of-tasks"></a>

### **在目标中添加目标**

通过 AddTarget，一个目标可在另一个目标内部执行。目标将按添加顺序执行。

示例：

```C#
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

TargetA 执行顺序为：

1. 构建任务；
2. TargetB 目标；
3. JustAnExample 方法。

### **在不同目标中复用任务集**

下例展示了如何在不同目标中复用任务集（reuse set of tasks）：

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

### **在 foreach 循环中为目标添加任务**

下例展示了如何在 foreach 循环中为目标添加多个任务：

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

例中，示例程序将为每个项目执行 Pack 任务。

<a name="Group-task"></a>

### **分组任务，并应用 When、OnError 和 Finally**

- 在任务组上使用 When 子句有条件地执行任务。

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

- 在任务组中使用 Finally：onFinally 的行为与 try-catch-finally 中的 Finally 相同。

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

- 在任务组中使用 OnError：可以在组中任意一个任务发生错误时执行一些自定义操作。

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

### **任务、自定义代码与依赖项的异步执行与并行执行**

<ul>
<li>
可通过 AddTaskAsync 或 AddCoreTaskAsync 方法异步执行任务。

</li>
<li>
可使用 DoAsync 方法异步执行自定义代码。

</li>
<li>
可通过 DependsOnAsync 方法异步执行依赖项。

</li>
</ul>
在下例目标中并行执行三个任务。

```C#
session.CreateTarget("run.tests")
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName3"));
```

异步方法和同步方法也可以混合使用。

```C#
session.CreateTarget("async.example")
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .AddTaskAsync(x => x.NUnitTaskForNunitV3("TestProjectName1"))
    .Do(SomeCustomMethod)
    .DoAsync(SomeCustomAsyncMethod2)
    .DoAsync(SomeCustomAsyncMethod3);
```

上面代码中，将首先异步执行两个 nunit 任务，并等待两个任务完成；随后将同步执行 SOmeCustomMethod，执行完后再并行执行 SomeCustomAsyncMethod2 和 SomeCustomAsyncMethod3。

#### 在异步执行的任务和目标中顺序打日志

通常来讲，在异步或并行执行多个任务时，日志是不可读（not readable）的。这就是为啥 FlubuCore 在异步任务中提供顺序记录（sequential logging）的原因。你可以在目标上使用 `.SequentialLogging(true)` 来启用，且必须放在异步任务/目标依赖项之前，否则日志就不是顺序的了。

```c#
context.CreateTarget("Test")
        .SetAsDefault()
        .SequentialLogging(true)
        .AddCoreTaskAsync(x => x.Pack())
        .AddCoreTaskAsync(x => x.Pack())
        .DependsOnAsync(test2, test3);
```

在 FlubuCore runner 中并行执行的目标在默认情况下是顺序记录日志的。

`flubu target1 target2 --parallel`

<a name="Other-features"></a>

### **其它功能**

#### 目标功能

- SetAsDefault 方法：当应用于目标时，如果在使用 runner 运行脚本时没有指定目标，则默认运行该目标；
- SetAsHidden 方法：当应用于目标时，目标将不会被显示在帮助信息中，并且它只能作为其它目标的依赖项来运行；
- Must 方法：设置必要条件，该条件必须满足，不然在任务执行之前目标就会执行失败。
- Requires 方法：在 `required` 方法中指定的参数不能为 `null`，否则在执行任务前就会失败。

#### 上下文功能


- Log：`context.LogInfo("Some Text2", ConsoleColor.Blue);`；
- GetVsSolution：获取解决方案和项目信息 `context.GetVsSolution();`。
- GetFiles: 在选项中使用 Glob Pattern 模式筛选指定目录获得文件 `context.GetFiles(OutputDirectory, "*.nupkg");`；
- GetDirectories: 在选项中使用 Glob Pattern 模式从制定目录中获取目录 `context.GetFiles(OutputDirectory, "*.nupkg");`；
- GetEnviromentVariable 方法：根据名称（name）获取环境变量 `context.GetEnvironmentVariable("someVariable");`；

<a name="Run-any-program"></a>

#### 使用 RunProgramTask 在构建脚本中运行程序或命令

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

Linux 的例子：

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

### **构建属性**

你可以通过属性上的特性（attribute）或 `ConfigureBuildProperties` 方法（以前的方法）定义多个构件属性，在不同的任务和自定义代码中共享它们。

下例展示了如何在各种目标（Target）/任务（Task）间共享解决方案文件名和配置。

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

另外：

```C# 
    [BuildProperty(BuildProps.BuildConfiguration)]
    public string BuildConfiguration { get; set; } = "Release";
```
如果解决方案文件名和路径不能通过构建属性特性（build property attributes）来设置，那么就必须在每个任务中分别进行设置，就像是：

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

#### 预定义的构建属性

一些构件属性已被预定义，可以通过接口获取：

`context.Properties.Get(PredefinedBuildProperties.OsPlatform);`

可用的预定义构件属性有：

- OsPlatform
- PathToDotnetExecutable
- UserProfileFolder
- OutputDir
- ProductRootDir

所有这些构建参数都可被覆盖。

<a name="Script-arguments"></a>

## **向构建脚本属性传递命令行参数、JSON 配置文件设置或环境变量**

可通过在属性上打 FromArg 特性的方式向构建脚本属性传递命令行参数、JSON 配置文件的设置或环境变量。

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

FromArg 特性第一个参数（parameter）是参数键（argument key）。第二个参数用于在 flubu runner 中显示属性的帮助描述。实际上在属性上打特性并不是必须的，如果你没有添加这个特性，那么参数键（第一个参数）会与属性同名，属性的帮助信息不会显示在构建脚本的 runner 上。

支持的属性类型有：string、boolean、int、long、decimal、double 和 DateTime。

<a name="Command-line-argument"></a>

### **向构建脚本参数传递命令行参数。**

`Dotnet flubu Deploy.Example -sn=true`

<a name="json-configuration-file"></a>

### **向构建脚本传递 JSON 配置文件的设置**

- 在 FLubu runner 所在的目录下创建 FlubuSettings.json 文件；
- 以 JSON 格式的方式向文件中添加参数的键和值；
- 对于上面的例子，JSON 文件将看上去是这样子的：
  `json {“sn”：true，“SomeOtherKey”：“SomeOtherValue”}`
- 对于不同的环境（如开发、测试和生产环境），通常有不同的配置。只需创建不同的 JSON 文件 `FlubuSettings.{Environment}.Json`，并在需要的机器上[设置环境变量](https://andrewlock.net/how-to-set-the-hosting-environment-in-asp-net-core/) 'ASPNETCORE_ENVIRONMENT' 即可；
- 还可以按机器名 `FlubuSettings.{MachineName}.Json` 创建 JSON 配置文件，如果文件中的 MachineName 与本机机器名匹配，Flubu 将自动从该文件中读取设置。

<a name="enviroment-variable"></a>

### **向构建脚本传递环境变量**

还可以通过环境变量设置脚本参数。环境变量前必须有前缀 `flubu_`。

对于上面的例子，你可以通过 Windows 命令行工具添加系统环境变量： `set flubu_sn = true`。

<a name="Arguments-pass-through-to-tasks"></a>

## **向任务传递控制台参数、JSON 配置文件的设置，以及基于 ForMember 的环境变量。**

还有一种更为复杂的方法来给任务传递控制台参数、设置和环境变量：

```C#
protected override void ConfigureTargets(ITaskContext context)
{
   context.CreateTarget("compile")
       .AddTask(x => x.CompileSolutionTask()
           .ForMember(y => y.SolutionFileName("someSolution.sln"), "solution", "The solution to build."));
}
```

- 第一个参数是需要传递的方法或属性的参数，如果在运行构建脚本时没有指定参数，则使用默认值；
- 第二个参数是参数键（argument key）；
- 第三个参数是可选的，在目标的帮助中显式帮助信息。如果参数没有设置，则显示默认生成的帮助。

`Dotnet flubu compile -solution=someothersolution.sln`

<a name="Referencing-other-assemblies-in-build-script"></a>

<a name="Build-system-providers"></a>

## **构建系统提供者程序**

你可以获取不同的构建系统（如 Jenkins、TeamCity、AppVeyor、Travis 等）的多种信息，如构建。提交等。

```C#
protected override void ConfigureTargets(ITaskContext context)
{
    bool isLocalBuild = context.BuildSystems().IsLocalBuild;
    var gitCommitId = context.BuildSystems().Jenkins().GitCommitId;
}
```

<a name="Before-After"></a>

## **构建事件**

- OnBuildFailed 事件：

```c#
public class BuildScript : DefaultBuildScript
{
    protected override void OnBuildFailed(ITaskSession session, Exception ex)
    {
    }
}
```

- 在目标执行前后执行的事件：

```c#
protected override void BeforeTargetExecution(ITaskContext context)
{
}

protected override void AfterTargetExecution(ITaskContext context)
{
}
```

- 在构建执行前后执行的事件：

```c#
protected override void BeforeBuildExecution(ITaskContext context)
{
}

protected override void AfterBuildExecution(ITaskSession session)
{
}
```

<a name="partial-class"></a>

## **脚本中的部分类和基类**

如果部分类（partial classes）和基类（base classes）位于同一个目录下，则会自动加载它们；否则，必须使用 [Include 特性](../referencing-external-assemblies#adding-other-cs-files-to-script)来添加。
