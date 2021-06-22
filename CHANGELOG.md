## FlubuCore 6.1.4.0
- Fixed load of targets from target attribute in interactive mode.
- Adds Flubu tool type to jenkins configuration options. 
- Improved duplicated target error message.

## FlubuCore 6.1.3.0
- Fixed Interface compilation in script when interface was in a namespace.
- (Jenzy) PackageTask: Added option to DisableLogging of copied files. 

## FlubuCore 6.1.2.0
- Fixed WaitForServiceStopTask, invalid cast.

## FlubuCore 6.1.1.0
- Support for C# 9.0 features in flubu scripts.

## FlubuCore 6.1.0.0
- (albyrock87) Introduced LogLevel instead of NoOutputLog / NoLog on external process tasks. 
- (albyrock87) Added git fetch task
- (albyrock87) Added git clean task
- FlubuTemplating: Added option to skip files to be copied from a template
- Jenkins configuration generation: Added AnsiColor option
- Jenkins configuration generation: Added missing step block in stage block.
- Jenkins configuration generation: Added proper command exectuor when adding flubu target to a step in pipeline. 
- Jenkins configuration generation: Target without tasks are not added anymore as steps in pipeline.
- Jenkins configuration generation: Wraps executed command into single quote.
- (Albyrock87) Fixed git clone syntax
- Fixed directory suggestions in interactive mode.
- TemplateReplacementTokenTask: Readed values from console are trimed now. 
- FlubuCore.Tool for .net 5.0
- Fixed FlubuCore.Tool nuget publishing

## FlubuCore 6.0.0.0
- CI configuration generation for azure pipelines, github actions, jenkins travis and appveyor. More can be read at https://flubucore.dotnetcore.xyz/CI-Generation/
- Simplified creation of new build scripts with FlubuCore templates. More can be read at https://flubucore.dotnetcore.xyz/flubu-templates/
- .net 5.0 support
- new analyzers for Build property attributes and task attributes
- Directories and files suggestions for `flubu setup` command 
- Dropped support for .net standard 1.6
- (albyrock87) Added Gitlab CI variables to BuildServer.
- (seanstride)removed obsolete APIs 
- (jenzy)CreateZipPackageFromProjectsImplementation should pass destinationRootDir to PackageTask
- (huanlin)Show working folder with full path in logs
- (huanlin)Wrong log info about executing npm process
- UnzipTask: returns list of extracted files.
- UpdateNetCoreVersionTask: Added option to Update all projects version in solution 
- ReplaceTokenTask: Don't replace token if empty value is entered.
- DotnetNugetPushTask: Added SkipDuplicate option
- Build server jenkins: Addded IsPullRequest property.
- WebApi: Imporeved logging of exceptions on web api execute script task.  

## FlubuCore 5.1.8.0
- Added ServiceStatus And ServiceStop windows service control tasks.
- NugetPublishTask: Packages can be now published with version quality suffix.

## FlubuCore 5.1.6.0
- Added some more common system assemblies so they don't need to be referenced through assembly attribute in scripts.

## FlubuCore 5.1.5.0
- Fixed resolving of nuget packages when dependency is not found
- Fixed resolving of nuget packages when dependecy is FlubuCore.

## FlubuCore 5.1.3.0
- dotnet command now works when full path to dotnet executable is not found.
- GitCheckout task: Fixes NewBranch (-b) and NewBranchWithReset(-B) option.
- GitCheckout task: Branch name is now optional.
 
## FlubuCore 5.1.1.0
- Improved build summary: added status and duration for each target
- When .flubu file is empty Flubu  looks for scripts, csprojs at default locations relative to .flubu file path
- Added build properties type validation
- Unified BuildVersion build properties
- predefined build dir and output dir is now combined with root directory
- Changed extension method names for RootDirectory and OutputDirectory
- Moved full .Net specific build property(BuildProps) constants to seperate class (DotNetBuildProps)
- Fixed bug: predefined build property can now be overriden through BuildProperty attribute
- Fixed bug: RootDirectory is not set on linux when using .flubu file
- Interactive mode: command history is not reset anymore on load / reload script bug

## FlubuCore 5.0.4.0
- dotnet pack task: fixed explicit set of project argument when solution file name is set in build properties.
- Fixed build script locator when .flubu file was empty.
- Fixed RunProgramTask when ProudtRootDir is not set.
- Target "task" is now hidden
- Interactive mode: Fixed simple interactive mode
- Interactive mode: fixed a bug where executing dotnet does not executes dotnet
- Interactive mode: Fixed a bug where execution of external process had wrong working directory in interactive mode

## FlubuCore 5.0.3.0
- dotnet clean, restore, build, test, pack publish tasks: Fixed insert of duplicated project argument when project path starts with /
- Fixed reporting of unspecified build script
- RunProgramTask: Improved log of executed command

## FlubuCore 5.0.2.0
- Execution of tasks with attributes on property

    ```c#
    [FetchBuildVersionFromFile]
    public BuildVersion BuildVersion { get;  }
    ```

    ```c#
    [GitVersion]
    public GitVersion BuildVersion { get;  }
    ```

    This allows  access version information in ConfigureTarget which was not possible if versioning task was executed for example as         target dependency
    ```c#
    protected override void ConfigureTargets(ITaskContext context)
    {
            context.CreateTarget("Build")
                .AddCoreTask(x => x.Build().FileVersion(BuildVersion.Version));
    }
    ```

- Get and Set build properties through attribute 

    Instead of setting build property through IBuildPropertiesContext  
    ```C#
     protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
     {
            context.Properties.Set(BuildProps.BuildConfiguration, "Release");
     }
    ````
    it is possible to set it through attribute

    ```C#
        [BuildProperty(BuildProps.BuildConfiguration)]
        public string Configuration { get; set; } = "Release";
    ````

    Or

    ```C#
        [BuildConfiguration]
        public string Configuration { get; set; } = "Release";
    ````

- Pack FlubuCore script as global tool or execute it directly through console app.

    ```c#
        public class Program
        {
            public static void Main(string[] args)
            {
                var engine = new FlubuEngine();
                engine.RunScript<MyScript>(args);
            }
        }
    ```

- Introduces FlubuCore modules

   It is possible change FlubuCore internal logic by  replacing FlubuCore modules through DI. For example logging, target runner, task      context etc.. could be changed. 

   ```C#
   public class BuildScript : DefaultBuildScript
   {
     public override void ConfigureServices(IServiceCollection services)
     {
        services.Replace<IFlubuSession, MyFlubuSession>();
     }

     public override void Configure(ILoggerFactory loggerFactory)
     {
         loggerFactory.AddProvider(new MyLoggerProvider());
     }
   }
   ```
   
   - Added DependeceOf to target
   
   ```C# 
    protected override void ConfigureTargets(ITaskContext context)
    {
           var b = context.CreateTarget("B")
                   .DependenceOf(A);
    }
   ``` 
   
- FlubuCore web api for .net core 3.1
- FlubuCore web api: Dropped support for net core app 1.1
- Store json configuration file path to .flubu file
- New version of dotnet.cli.utils
- FlubuCore now places compiled build script into the location where build script is located.
- (Breaking change) OutputDirectory property is now obsolete in DefaultBuildScript
- RunProgramTask: Sets working folder to product root directory.
- DotnetPublishTask: Added PublishSingleFile, PublishTrimmedFile, PublishReadyToRun
- DotnetBuildTask: Added output
- Added Configuration Runtime, Framework enum
- CreateWebApplicationTask: Fixed AddMimeType
- Changed ConfigureBuildProperty from abstact method to virtual method.
- Flubu Sets root dir to the location of the flubu file
- improved flubu setup texts
- Fixed help alligment
- Fixed broken lins to help in build script loader error messages
- Fixed parsing of .flubu file when it is empty
- Fixed RootDirectory when used in ConfigureTargets in interactive mode.
- Fixed set of project name from flubu build properties when options with '/' is included in args
- Fixed flubu setup when build script is not found 
- Fixed set of project name from flubu build properties when options with '/' is included in args
- Interactive mode: History of executed commands doesn't work #260
- Interactive mode does not fail anymore when defined build property is accessed in ConfigureTargets
   


## FlubuCore 4.3.7.0
- DotnetBuildTask, DotnetPackTask, DotnetPublishTask: VersionSuffix option key is not added anymore if it is null or empty.
- Changed properties accessors on DefaultBuildScript so they can not be overriden through passed arguments.
- Interactive mode: Fixed load and reload of build script.
- Interactive mode: Fixed load and reload of build script when build script location is specified by user (-s)
- Interactive mode: Fixed hint highlights for script properties.
- Interactive mode: Fixed suggestions for properties with more than one key.
- Interactive mode: Help now shows build script global arguments.
- Interactive mode: Added logging when build script is not found. 
- Added some more default build script locations.
- Added some more default csproj locations.
- Flubu help: Adds colon betwen target name and target description even if description is empty.

## FlubuCore 4.3.5.0
- BuildVersionWithQuality adds back the dash separator if necessary
- BuildVersionWithQuality version field count is now optional
- Added Implicit string operator for FileFullPath

## FlubuCore 4.3.4.0
- UpdateNetCoreVersionTask: Version field count is now configurable
- UpdateNetCoreVersionTask: Option to add PackageVersion to csproj.
- UpdateNetCoreVersionTask: Fixes set of version suffix when setting version to more than one csproj.

## FlubuCore 4.3.3.0
- Fixes SetBuildVersionWithSuffix which also caused bug in UpdateNetCoreVersionTask
- Fixes spacing when displaying target dependencies in help
- Interactive mode: Fixes suggestions when suggestion key is null

## FlubuCore 4.3.1.0
- Added GetFiles with globing option to task context
- Added GetDirectory with globing option to task context
- Target: Added Requires method which checks if specified parameter is null. If it is null target execution fails before any tasks are executed.
- Option to run flubu script from other than root dir (only when .flubu file is found)
. Get project root directory from task context and DefaultBuildScript
- Get Output directory from task context and DefaultBuildScript
- Flubu setup: option to enter script csproj location even if build script location is found.
- GetVsSolution: Easier access to various project information
- GetVsSolution: Filter projects by name with glob pattern
- AppVeyor: Added branch name.
- AppVeyor: Added IsTag 
- AppVeyor: Added TagName
- Fixed include directory attribute to load only cs files
- Overidable argument now checks if same short/long key was already added and overrides appropriate one.
- Added GetFiles To FullPath class. 
- Target help now shows target dependencies in brackets
- Target help now shows default target with orange color
- Global script arguments of enum type have all available values listed in help now
- Added some more default build script locations.
- Interactive Mode: Suggetions for parameter values of enum type
- Interactive mode: Allows multiple tab key presses on sugestions(goes to next suggestion)
- Interactive mode: When listing files file length is now formated.
- Interactive mode: when pressing space sugestion is now hidden. 
## FlubuCore 4.2.8.0
- FlubCore global tool for .net core 3.0
## FlubuCore 4.2.7.0
- FromArg attribute now supports multiple keys. Keys must be seperated with |
- DotnetCoreTasks: Fixed dotnet executable path for OSX
- LoadSolutionTask: Fixed path to solution and project files in linux and mac
- Interactive mode: Fixed crash after clear screen (cls)
- Interactive mode: Fixed crash when external process is not found when executing target
- Interactive mode: Fixed delete character in console
- Interactive mode: Fixed space when not typing at the end
- Interactive mode: Improved error handling when target fails. 
 

 
## FlubuCore 4.2.4.0

- Added dotnet msbuild task with tab completion in interactive mode.
- Added new docker tasks and updates existing ones by regenerating them from documentation
- Added git branch task
- Added git merge task
- Added git extension on ITaskContext which contains local repository information. For example: ` context.Git().CurrentBranchName()`
- Added GitHubActions build system
- Added Azure pipelines build system
- Added some more jenkins helpes in coresponding build system
- Added some more travis helpers in coresponding build system
- Updated options on existing git tasks 
- Added missing dotnet nuget push options
- Added missing dotnet publish options
- Added missing options to dotnet test
- PackageTask: AddDirectory now support directory filtering when subdirectories are packaged
- PackageTask: Added support for filtering by glob pattern
- FromArg attribute now supports enums
- Moved Chocolatey tasks from FlubuCore plugin to FlubuCore (possible braking change) - Just remove plugin if you are using it.
- Interactive mode: tab completion for all docker commands and options
- Interactive mode: Directory completion with tab key on cd command
- Interactive mode: Added cls internal command for clearing the screen
- Interactive mode: Improved tab completion for external processes
- Interactive mode: hints are now case insensitive
- Interactive mode: Different colors for different types of suggestions
- Interactive mode: removed duplicated shor and long options sugestions 
- Interactive mode: move to path root with 'cd...'
- Interactive mode: fixed bug where flubu in interactive mode crashed when clearing old help
- Interactive mode: Fixed target detailed help
- Interactive mode: Removes new lines and unecessary whitespaces from help at the bottom.
- Interactive mode: Task execution errors are now logged.
- Interactive mode: Fixes cd.. so it doesn't go to the root directory on disk.
- docker tasks: optional options are now as optional parameters.
- External process task do not log RunProgramTask execution info anymore.
- Flubu cli tool and global tool works now with only .net core 3.0 installed on machine.  
- Fixed various bugs in chocolatey task. 
- Fixed docker options key prefix.
- Fixed package task when no files exists to zip
- Fixed a bug where interactive mode hang on illegal command.
- minor logging fixes
- fixed null or empty parameters in WithArguments 
- External process tasks: Improved value required error message when passing argument with value.
- Internal: changable key value separator in WithArgumentKeyFromAttribute method
- Internal: Implemented FlubuConsole task generator
- Internal: Various task generator improvements


## FlubuCore 4.1.2.0
- Interactive mode: Option to execute external processes such as dotnet, git, docker... 
 
- Options completion in interactive mode for tasks that run's external process

currently tab completion is available for all dotnet commands, most of git commands, coverlet, sqlcmd, gitversion.
It is planned that all docker, azure, npm, octopus commands will support tab completion in near feature.
![FlubuCore interactive mode](https://raw.githubusercontent.com/dotnetcore/flubu.core/master/assets/FlubuCore_Interactive_mode_external_process.gif)


- Interactive mode: help for targets and options

![FlubuCore interactive mode](https://raw.githubusercontent.com/dotnetcore/flubu.core/master/assets/FlubuCore_Interactive_mode.gif)

- interactive mode: reload or load another script

You can load new script with following commands in interactive mode:
`load -s=newscript.cs`
`l -s=newscript cs`

You can reload script with the following command:
`reload`
`r`

- Override existing options or add additional options to tasks through console improvements

Let's say you have target (this is just simple dummy example)
```c#
context.CreateTarget("Example")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release"); 
    .AddCoreTask)x => x.Pack("ExampleProject");
You could just write in console
```

`flubu build -c=Debug`

flubu would execute

`dotnet build MySolution.sln -c Debug`

- interactive mode: navigation between folders 
- fixed terminal entered command. Flubu executed sugestion instead of actually entered command 
- FetchBuildVersionFromFile: Fixed default locations output
- Fixed additional options prefixes
## FlubuCore 4.0.3.0
- FlubuCore interactive mode which offers target tab completition, options tab completition, toogle targets and options, executed commands history and more.   

![FlubuCore interactive mode](https://raw.githubusercontent.com/flubu-core/flubu.core/master/assets/FlubuCore_Interactive_mode.gif)

- Targets: Execute another target within target with AddTarget.

``` C#
    protected override void ConfigureTargets(ITaskContext context)
    {
       var exampleB = context.CreateTarget("TargetB")
            .Do(Something);

       context.CreateTarget("TargetA")
           .AddCoreTask(x => x.Build())
           .AddTarget(exampleB)  //Target is executed in the order it was added
           .Do(JustAnExample);
    }

    public void JustAnExample(ITaskContext context)
    {
        ...
    }
```
- Target: Add tasks to target with a foreach loop.
```c#
  protected override void ConfigureTargets(ITaskContext context)
  {
         var solution = context.Properties.Get<VSSolution>(BuildProps.Solution);

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

- Override existing options or add additional options to tasks through console

<sup>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Let's say you have target</sup>
```c#
context.CreateTarget("Build")`
    .AddCoreTask(x => x.Build("MySolution.sln").Configuration("Release"); 
```

<sup>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;and you wan't to build solution in debug configuration.</sup>

<sup>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;You could just write in console</sup>

`flubu build /o:configuration=Debug`

<sup>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;or</sup>

`flubu build /o:c=Debug`

<sup>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;flubu would execute </sup>

`dotnet build MySolution.sln -c Debug`
- sequentiall logging in asynchronus executed tasks and targets.

```c#
context.CreateTarget("Test")
        .SetAsDefault()
        .SequentialLogging(true)
        .AddCoreTaskAsync(x => x.Pack())
        .AddCoreTaskAsync(x => x.Pack())
        .DependsOnAsync(test2, test3);
```

- New plugins are available: Chocolatey, Gitter and Slack plugin.
- Added Get solution infromation as task context extension
- Added support for multiple Musts on target
- Logs have now indentation for better readability.
- Logs have now timemark (actions that takes more than 2sec).
- Improved build summary in logs.
- Loged build finish time and build duration
- Fixed GitVersionTask
- Targets: Must now accepts optional error message parameter.
- UpdateNetCoreTask can now write version quality(version suffix)
- FetchBuildVersionFromFile can now fetch version quality(version suffix).
- FetchBuildVersionFromFile improved logging.
- Added options to set versions in dotnet tasks (dotnet build, dotnet pack, dotnet publish)
- LoadSoluationTask returns solution information
- Added WithArgument to IRunProgramTask interface
- Fixed check of unknown targets.

## FlubuCore 3.2.1.0
- Fixed build status
- Improved message when build script is not found.

## FlubuCore 3.2.0.0
- Added coverlet task ```.AddCoreTask(x => x.CoverletTask("assembly.dll")```
- Adds flubu setup where you can set location of the build script and project file. run ```flubu setup```
- Added When condition to all tasks.
```c#
 var compile = context
            .CreateTarget("compile")
            .SetDescription("Compiles the VS solution")
            .AddCoreTask(x => x.Build().Configuration("Release").When(
            () =>
            {
                return context.BuildSystems().IsLocalBuild;

            }, task => { task.Configuration("Debug"); }));
 ```
- Fixed bug where nuget and assemlby references were not loaded if csproj didnt have both of them
- Adds OnBuildFailed event.
```c#
public class BuildScript : DefaultBuildScript
{
    protected override void OnBuildFailed(ITaskSession session, Exception ex)
    {
    } 
}
 ```
- Adds before and after target execution events.
```c#
    protected override void BeforeTargetExecution(ITaskContext context)
    {
    }

    protected override void AfterTargetExecution(ITaskContext context)
    {
    }
```    
- Adds before and after build execution events.
```c#
    protected override void BeforeBuildExecution(ITaskContext context)
    {
    }

    protected override void AfterBuildExecution(ITaskSession session)
    {
    }
 ```
- Improved nunit tasks fluent intefaces.
- Added skipped target dependencies and tasks logging.
- Publicly exposed task name.
- fixed one of the default build script csproj locations.

## FlubuCore 3.1.2.0
- Fixes Must on target fluent interface.
- Fixes script when using partial classes. Script failed in some scenarios.
- script allows includes of other cs files in partial classes.
- System.Drawing.Primitives assembly reference doesn't need to be referenced exlicitly anymore in script uses collored logging (issue was only present when target .net core framework)
- Improved some script error messages.

## FlubuCore 3.1.1.0
- Fixes loading of nuget packages that don't have target framework specified.
- FetchBuildVersionFromFileTask: Improves fetching of version from file by allowing version to be in any line not just first.
- FetchBuildVersionFromFileTask: Adds default project version file locations.
- FetchBuildVersionFromFileTask: Adds option to remove prefix from version.
- FetchBuildVersionFromFileTask: Adds option to allow any suffix. 
- Improves error messages when cs files that are used in script are not included.
- Improves error messages when script assembly references are not loaded.
- Added Null and empty target name validation.

## FlubuCore 3.1.0.0
- Added IncludeFromDirectoryAttribute: Attribute adds all .cs files from specified directory. With second optional parameter you can include subdirectories. 

```c#
[IncludeFromDirectory(@".\Helpers")]
public class BuildScript : DefaultBuildScript
{
}
 ```
- AssemblyFromDirectoryAttribute: When added on script class FlubuCore should add all assemblies from specified directory to script.

```c#
    [AssemblyFromDirectory(@".\Lib")]
    public class BuildScript : DefaultBuildScript
    {
    }
```
- Load base script class automatically. Must be in same directory as script.
- Improved collored console logging by wrapping strings of the output in ANSI codes that instruct the terminal to color the string based on the interpreted code.
- Allow namespaces in included cs files. Executing script does not fail anymore if included cs file contain namespace.
- Disable collored logging with attribute or script argument. 

## FlubuCore 3.0.2.0
- Fixed attribute "directives"

## FlubuCore 3.0.1.0
- Fixes restoring and loading of nuget packages with old csproj
- Stylecop nuget packages are not loaded anymore
- build status is now logged with color.
- Target and task information is now logged with color.

## FlubuCore 3.0.0.0
- Resolve nuget packages from build script csproj file automatically. No need for directives in build scripts anymore when executing script that is in project. For standalone scripts you still need directives.
- Load referenced libraries from build csproj file automatically. No need for directives in build scripts anymore when executing script that is in project. For standalone scripts you still need directives.
- All nuget dependencies are loaded automatically.
- Added GitVersionTask: GitVersion is a tool to help you achieve Semantic Versioning on your project. [Documentation](https://gitversion.readthedocs.io/en/latest/)

```C#
  context.CreateTarget("Example")
         .AddTask(x => x.GitVersionTask());
```
- Automatic load of build script partial classes. Partial classes have to be in same directory as build script.
- Automatic update of FlubuCore web api to new version. Just navigate to /UpdateCenter
- Added small web app to FlubuCore web api for executing scripts. Navigate to /Script
- Pass console and config arguments to targets defined with attribute
```C#
 [Target("ExampleTarget", "Default string")]     
 [Target("ExampleTarget2", "Default2 string")]
 public void Example(ITarget target, [FromArg("e")]string Example)
 {
     target.Do(x => { x.LogInfo(boo });
 }
```

`dotnet flubu ExampleTarget -e=Hello`

or

`dotnet flubu ExampleTarget -Example=Hello`

- load assembly references, nuget references through script class attributes. Alternative to directives.

```C#
[Assembly(@".\packages\Newtonsoft.Json.9.0.1\lib\net45\Newtonsoft.Json.dll")]
[NugetPackage("Newtonsoftjson", "11.0.2")]
[Reference("System.Xml.XmlDocument, System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
public class BuildScript : DefaultBuildScript
{
}
```
- Added Must method to target fluent interface. Condition in must will have to be meet otherwise target execution will fail before any task get executed.

- Option to execute custom code before each target execution(target dependencies are excluded). Just override following methods:

```C#
 protected override void BeforeTargetExecution(ITaskContext context)
  {
  }

  protected override void AfterTargetExecution(ITaskContext context)
  {
  }
```

- Colored console logging

```C#
  public void Example(ITaskContext context)
    {
        context.LogInfo("Some text", ConsoleColor.DarkGray, ConsoleColor.DarkMagenta);
        context.LogInfo("Some Text2", ConsoleColor.Blue);
        context.LogError("Error", ConsoleColor.Blue);
    }
```

 - FetchVersionFromExternalSourceTask now supports following build systems : Bamboo, Bitrise, ContinousCl, Jenkins, TFS, TeamCity, TravisCI.

- CompileSolutionTask: support for VS2019 
- Restoring of nuget packages has fallback to msbuild now if dotnet core sdk is not installed
- New Config attribute: Disable load of script references from csproj
- New Config Attribute: Always recompile build script
- Expose the output of ExternalProcessTaskBase
- Fix: Specifing flubu script path in config file does not work
- Fix: BuildScript assembly is not recompiled when included cs files are changed
- Fix: Added interactive method to task fluent interface.
- Fixed switched capture output and capture error output
- Fixed GetRemoveFileTask base methods (null reference exception)
- Fixed GitPushFileTask base methods (null reference excetpion).
- Fixed GitAddTask base methods (null reference excetpion).
- External process task return type is now generic
- script key is now case insensitive in file configuration.
- switched from Microsoft.Extensions.CommandLineUtils to McMaster.Extensions.CommadLineUtils
- Minor visual improvements when displaying help.
- Added Net462 Web api deploy package for x86
- Added some more default build script locations.

## FlubuCore 2.14.0

- Add support for interactive task members. 

Example:

        protected override void ConfigureTargets(ITaskContext context)
        {
             context.CreateTarget("Example")
                    .AddTask(x => x.CompileSolutionTask()
                        .Interactive(m => m.BuildConfiguration("default value if interactive mode disabled or value is 
                         passed as argument."), consoleText: "Enter build configuration:"));

            context.CreateTarget("Example2")
                .SetAsDefault()
                .Do(Example2, 
                     "default value if interactive mode disabled or value is passed as argument", 
                      options => { options.Interactive(m => m.Param, "-test", consoleText: "Enter example value to print to console:"); });
        }

        private static void Example2(ITaskContext context, string value)
        {
            context.LogInfo($"Entered: {value}");
        }

If u run target Example2 from console u will be prompted by Flubu to enter value which will be then printed to console.

- Flubu now supports cleanup actions when hitting control + c.. If u want that cleanup actions are performed You have to explicitly set parameter in task Finally method or in group parameter.

Bellow is example in which FlubuCore performs specified cleanup actions when hitting control + c or control + break (two optional parameters with true value are added)

     context.CreateTarget("run.postgres")
            .SetAsDefault()
            .Group(
                target =>
                {
                    target
                        .AddTaskAsync(task =>
                            task
                                .DockerTasks()
                                .Run("postgres:10.5", string.Empty)
                                .Finally(innerContext => innerContext.LogInfo("Testing1"), true));
                }, innerContext => { innerContext.LogInfo("Testing2"); },
                cleanUpOnCancel: true);  
                
- Improves logging on web api client tasks.
- (Possible breaking change)Removed obsolete methods from some tasks
- Adds option to set destination package folder in CreateZipPackageFromProjcet tasks. Currently only default destination folder could be used.

## FlubuCore 2.13.0.0
- Added docker generated tasks
- (Breaking changes) Old Docker task's that were previously manually implemented were overwriten with generated task.
  - Docker tasks that used property of type List<> were replaced with params. 
  - Some method names in docker tasks were renamed to the same name as it is option name in the docker command.
- Improved target fluent interface intelisense by adding methods from base interface.
- GitBranch information can be readed for jenkins build system. context.BuildSystems().Jenkins().GitBranch;
- WebApi increased max size limit of uploaded content

## FlubuCore 2.12.0 
- Added Git tasks:Clone, Pull Add, Commit, Push, Tag, RemoveFilesTask
            
          context.Tasks().GitTasks().Commit();

- Added Docker tasks: Build, Run, Stop, RemoveContainer, RemoveImage   

          context.Tasks().DockerTasks().Build(".");

## FlubuCore 2.11.0
- [FlubuCore cake plugin](https://github.com/flubu-core/FlubuCore.CakePlugin) which allows you to use any cake addin in FlubuCore.
- Added FlubuCore specific code analyzers. Added target parameter analyzers when specifing target's with Target attribute. Added FromArg attribute analyzer.
- Greatly improved performance for .net core.
- Improved nuget package resolving performance.

## FlubuCore 2.10.0
- Json configuration file can now be specified by machine name. FlubuSettings.{MachineName}.Json
- Added WaitForDebugger task context extension.

## FlubuCore 2.9.0
- Added support for referencing a nuget package in build script. https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Referencing-nuget-packages
- Added support for debugging the script by attaching flubu process to the debuger. https://github.com/flubu-core/flubu.core/wiki/6-Writing-build-script-tests,-debuging-and-running-flubu-tasks-in-other-applications

## FlubuCore 2.8.2 

- FlubuCore is now available as .net core global tool. `dotnet tool install --global FlubuCore.GlobalTool`
- Flubu dotnet cli tool and web api is now available for .net core 2.1.
- Console arguments, configuration properties, enviroment variables can now be passed to script properties with FromArg attribute. Property doesn't need to have attribute. Argument key in that case is same as the property name.

      public class SimpleScript : DefaultBuildScript
      {
        [FromArg("-sn", "If true app is deployed on second node. Otherwise not.")]
        public bool deployOnSecondNode { get; set; }

        protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
        {
        }

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

You could then pass argument to property like so:

`Dotnet flubu Deploy.Example -sn=true`

- Target's can now be defined with attribute on method.

        [Target("targetName", "a", "b")]
        [Target("targetName2", "c", "d")]
        [Target("targetName3", "e", "f")]
        public void Example(ITargetFluentInterface target, string source, string destination)
        {
                target.AddTask(x => x.CopyFileTask(source, destination, true));
        }

## Flubu 2.7.0
- Added Xunit task - For running xunit tasks with xunit console runner.
- WebApi: Option to include flubu web api server logs into ExecuteScript response.
- WebApi: Option to include StackTrace to error response.
- Added Build system providers - You can acces various build, commit... information for various build systems (such as Jenkins, TeamCity, AppVeyor, Travis...) 

      protected override void ConfigureTargets(ITaskContext context)
      {
            bool isLocalBuild = context.BuildSystems().IsLocalBuild;
            var gitCommitId = context.BuildSystems().Jenkins().GitCommitId;
      }

-  Added conditonal task execution with when cluase on single task (see bellow for group of tasks)

       context.CreateTarget("Example")
            .AddTask(x => x.CompileSolutionTask())
            .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec"))
                .When(c => c.BuildSystems().Jenkins().IsRunningOnJenkins);

- Added finally block on single task. Finally block acts just like finally in try catch  (see bellow for group of tasks)

       context.CreateTarget("Example")
            .AddTask(x => x.CompileSolutionTask())
            .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec")
                .Finally(c => c.Tasks().DeleteFilesTask("pathtoNuspec", "*.*", true).Execute(c)));

- Added onError block on single task.  You can perform some custom action when error occures on single task(see bellow for group of tasks)

       context.CreateTarget("Example")
            .AddTask(x => x.CompileSolutionTask())
            .AddTask(x => x.PublishNuGetPackageTask("packageId", "pathToNuspec")
                .OnError((c, ex) => c.Tasks().DeleteFilesTask("pathtoNuspec", "*.*", true).Execute(context)));

-  Added conditonal task execution with When clause on group of tasks.

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

- Finally on group of tasks: Added onFinally block on group of tasks. onFinally acts just like finally in try/catch.

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
- OnError on group of tasks:  You can perform some custom action when error occures in any of tasks that are in group.

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

## Flubu 2.6.0

- Added option to add multiple tasks, dependencies With (anonimous) method to target with Do method.

        protected override void ConfigureTargets(ITaskContext context)
        {
            context.CreateTarget("deploy.PreProduction")
                .Do(Deploy, @"C:\Web", "AppPool_Preprod", "PreProduction");

            context.CreateTarget("deploy.Production")
                .Do(Deploy, @"d:\Web", "AppPool_Prod", "Production");
        }

        public void Deploy(ITargetFluentInterface target, string deployPath, string appPoolName, string enviromentName)
        {
            target.AddTask(x => x.IisTasks().ControlAppPoolTask(appPoolName, ControlApplicationPoolAction.Stop))
                .Do(UnzipPackage)
                .AddTask(x => x.DeleteDirectoryTask(deployPath, false).Retry(30, 5000))
                .AddTask(x => x.CreateDirectoryTask(deployPath, false))
                .AddTask(x => x.CopyDirectoryStructureTask(@"Packages\WebApi", deployPath, true))
                .AddTask(x => x.IisTasks().ControlAppPoolTask(appPoolName, ControlApplicationPoolAction.Start));
        }

## Flubu 2.5.0

- Added create HttpClient task.

## Flubu 2.4.0
- Added BuildScriptEngine for writing build script tests, build script debuging and executing flubu tasks in other .net (core) applications. See example: https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs

## Flubu 2.3.0
- Option to pass through arguments to tasks

        context.CreateTarget("ExampleTarget")
            .AddTask(x => x.CompileSolutionTask()
                .ForMember(m => m.BuildConfiguration("Release"), "c"));

        //// alternatively you can get argument value like this:
        string configuraiton = context.ScriptArgs["c"];

You can execute target with -c argument:

`build.exe ExampleTarget -c=Debug`

You can also set value in configuration file (flubusettings.json) or through enviroment variable. See https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals#Script-arguments for more info.

## Flubu 2.0.0

- Added support for multiple target execution in script runner.
- Added task for executing powershell scripts.
- Added NunitWithDotCover task.
- Added option to get predefined build properties - See build script fundamentals for more info.

1.9.0.0
- add support for setting verbosity and other logging properties for msbuild (Compile Task).  
- Improved ServiceControlTask inteface.
- Added CreateService task.
- All Task that uses external program now implements external process tasks interface(Improved usability).

1.8.1.0
- dotnet-flubu is now also build in .netcoreapp 1.0 target framework to support older versions.  

1.8.0.0
- added support for finding and using MSBuild 15.0 in builds.
- All tasks that uses external programs have now fluent interface to add custom arguments.
- Improved dotnet core task fluent interface.
- Added GetEnviromentVariable extension method to TaskContext.

1.7.3.0
- if not target specified and not default target, display help.

1.7.2.0
- added SleepTask.
- added Sc.exe task.
- added generic interface for external process tasks.

1.7.1.0
- Fixed script arguments pass through on flubu runner and dotnet-flubu cli tool.

1.7.0.5
- Fixed NoLog ITask interface.

1.7.0.4
- Option to disable task logging.

1.7.0.3
- Updated SqlCmdTask (do not escape args).

1.7.0.2
- merged replaceToken and replaceText tasks.

1.7.0.1
- added standard sqlcmd params.

1.7.0.0
- Added SqlCmd task with output capture support.
- Added capture output support for RunProgramTask.

1.6.9.1
- fixed compile task (correctly pass arguments to msbuild).

1.6.9.0
- updated compile task to support workingFolder and any params for msbuild.
- fixed il merge generation with libz container.

1.6.8.0
- Fixed references to system assemblies in .net 462 which caused stackoverflow exception in flubu.runner in atleast package task.

1.6.7.1
- Fixed PackageTask logging.

1.6.7.0
- Fixed retry on tasks.

1.6.6.1
- Fixed binding redirect for System.Security.Cryptography.Algorithms.
- Display flubu version info.

1.6.6.0
- Flubu is now build with .net core sdk 2.0 on build serve.

1.6.5.0
- fixed flubuCore runner binding redirects.

1.6.4.0
- Dotnet-flubu now targets netcoreapp1.1.

1.6.3.1
- Added SqlCmd task for executing MS SQL scripts.
1.6.3.0
- FlubuCore: Fixed generic parameters in Do Task.
- FlubuCore: Removed verbose switch for default NUnit 3 task. Issues with nunit 3.7.

1.6.2.0
- FlubuCore: Fixed web api set of base url when doing more that one request in one target.
- FlubuCore: Added option to add generic parameters to Do task. 

1.6.1.0
- FlubCore: All tasks have now retry mechanism option.
- FlubuCore: All tasks have now do not fail option.

1.6.0.0
- FlubuCore, WebApi: Added option to pasthrough arguments to targets and custom tasks.
- FlubuCore: Added upload script task.
- FlubuCore: Added delete packages task.
- FlubuCore: Fixed web api base url set through build propertis.
- WebApi: Added option to upload scripts to web api through web api method.
- WebApi: Fixed CommandArguments lifestyle.
- WebApi: Only scripts in script directory can now be executed.

1.5.2.0
- Fixed UploadPackageTask.
- web api ip white list access restriction.

1.5.1.0
- Added flubu web api GetToken task.
- Improved flubu web api UploadPackageTask and ExecuteScriptTask.
- Added web api documentation.

1.5.0.0
- Added FlubuCore web api for remote script execution.
- Added FlubuCore web api client.
- Added Flubu web api tasks to FlubuCore.

1.4.13.0
- Removed Client from FlubuCore.

1.4.12.0
- Upgraded nuget packages.

1.4.11.0
- Added generic DoNotFailOnError setting for tasks.

1.4.10.0
- Updated nuget packages.
- Fixed ControlAppPoolTask missing dependencie.

1.4.8.0
- Fixed version information in GenerateCommonAssemblyInfo task extension.
- GenerateCommonAssemblyInfo task extension has now GenerateCommonAssemblyInfo task action parameter.
- BREAKING CHANGE: Improved core pacakge extension tasks.
- Improved some others task extensions with task action parameter.

1.4.6.0
- Added do not fail option to run program task.

1.4.5.0
- Fixed dotnet core fluent interface for dotnet restore build and publish.

1.4.4.0
- Fixed dotnet core tasks (build, restore, clean, publish,) when no project name is defined. 

1.4.3.0
- BREAKING CHANGE updated flubu.core and flubu.runner .net framework from .net4.6 to .net.462.

1.4.2.0
- Added support for including other .cs files into buildscript with //#imp {PathToCsFile}.
- Added dotnet nuget push task.
- Added dotnet entity framework task.
- CompileSolutionTask: Improved Msbuild path locator Using Microsoft tool location helper now. Registry locator is now used as fallback .
- Solution name and configuration are now added form build props in DotNet specific tasks if not specified explicitly.
- PackageTask: Added option to disable logging of which files were filtered and copied.
- Updated flubu dependencies. No release candidates are referenced anymore. See https://bitbucket.org/zoroz/flubu.core/commits/cfeaec842a83dfd06f62c13aadd2b74496e47fa7 for more info.

1.3.11.0
- Updated Microsoft.Web.Administration from 10.0.0-rc1 to 10.0.0 used for iis tasks.
1.3.10.0
- CompileSolution task now supports specifing your own paths to Msbuild. If msbuild path is not specified or not found MsBuild is still searched at default locations. 
1.3.9.0
- SSH command capture output stream directly.
1.3.8.0
- SSH command task fixes.
1.3.7.0
- Added SSH support for entering password.
- Added support for executing multiple commands in one session.
1.3.6.0
- Added support for SSH. SshCommand and SshCopy tasks.
1.3.5.0
- CreateApplicationPool iis task: .Net clr version can now be set.
- All iis tasks have now fluent interface.
1.3.4.0
- Fixed iis task interfaces. They now contain Execute and ExecuteVoid methods.
- PackageTask - fixed issue when output zip file name contains more than one dot.
1.3.3.0
- Added support for external assembly loading by assembly relative path
- Target names are case insensitve now
- If target to be run does not exist help is now shown instead of default target being run.
- Build script can now contain namespace
1.3.2.0
- Added support for external reference based on type loading.
- Added support for external assembly loading by assembly full path.
- Do in Target is now a task.
- BREAKING CHANGE: Do is now executed in the order specified in build script and not anymore before all tasks.
- Added DoAsync to target: For asynchronus custom code execution.
- Added AddTaskAsync to target: For asynchronus task execution.
- Added DependsOnAsync to Target: For asynchronus target dependencies execution.
- All tasks have now ExecuteAsync method.
1.2.3.0
- Added explicit System.IO reference to Roslyn scripting engine.
1.2.2.0
- Added LogInfo and LogError to TaskContext.
- Added fluent interface to PublishNugetPackageTask.
- Added fluent interface to CopyDirectoryStructureTask.
- Added fluent interface to FetchBuildVersionFromFileTask.
- Added fluent interface to UpdateXmlFileTask.
- Added fluent interface to ReplaceTokensTask.
- moved packaging filters to it's own namespace.
- Added FlubuCore and dotnet-flubu nuget metadata.
1.2.1.0
- Minor fixes.
1.2.0.0
- Flubu.Runner now works without any manual config modifications. 
- Task fluent interface documentation.
- Added Dotnet specific tasks and extensions.
- BREAKING CHANGE: Splited TaskExtensions into CoreTaskExtensions and TaskExtensions.
1.1.10.0
- Updated nuget packages to latest version.
1.1.9.0
- Removed dotnet test -xml output parameter. It won't work under VS2017 RC.
1.1.8.0
- BREAKING CHANGE: Removed DependsOn by TaskExtensionsFluentInterface from TargetFluentInterface. Use BackToTarget instead on TaskExtensionFluentInterface.
1.1.7.0
- System tests.
1.1.6.0
- CompileSolutionTask - specific platform can be set now. Default is still AnyCPU.
- CompileSolutionTask - Custom arguments can be added now. 

1.1.5.0
- Added FlubuCore.Runner for .net 4.6.
1.1.4.0
N/A
