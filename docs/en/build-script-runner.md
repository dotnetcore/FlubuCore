

In .NET you run flubu build scripts with console application flubu.exe. Add Flubu.Runner nuget to get the console application.

In .NET core you run flubu build scripts with dotnet command line tool or global tool. How to install tools see [Getting started section]

### **Usage**

.NET core usage: `dotnet flubu {TargetToExecute} {Flubu options} {build script arguments}`

.NET core global tool usage: `flubu {TargetToExecute} {Flubu options} {build script arguments}`

.NET usage: flubu.exe `{TargetToExecute} {Flubu options} {build script arguments}`

#### Multiple target execution

.NET core usage: dotnet flubu {TargetToExecute} {TargetToExecute2} {TargetToExecute3...} {Options}

.NET usage: flubu.exe {TargetToExecute} {TargetToExecute2} {TargetToExecute3...} {Options}

Target's can be executed in parallel with added `-parallel` option 
### **Flubu options**

![N/A](img/FlubuHelp.png "Flubu help")

### **target help**

List all available targets:

`flubu help`

`dotnet flubu help`

Target specific help:

`flubu {TargetName} help`

`dotnet flubu {TargetName} help`

It displays all tasks with description that will be executed by target. It also displays which argument(with description) can be passed through to specific task in target. 

#### Specifiying which script Flubu should run.

- Easiest way is to put build script at one of the default locations (you can find list of default locations below). If it is located at one of the default locations FlubuCore will execute the script automatically `flubu {TargetName}`. 

- Second option is to specify script location with -s option `flubu {TargetName} -s={pathToScriptFile}` 

- Third option is to run `flubu setup` and specify script and csproj(optional) location in interactive mode.
 Flubu will store script and csproj location to `.flubu` file. if `.flubu` file is present FlubuCore will read location of the script and csproj file from that file. Additional benefit when storing location to `.flubu` file is 
 that you don't need to execute script at the root directory of the project. Meaning if your project is for example located at "c:\_git\myproject" you can execute script inside any subfolder of that location. The location where a 
 .flubu file is found will be used as the "work directory" during a build process, and a correct "work directory" is crucial to use relative paths in our build scripts.

### **Default build script locations**

- "Build.cs"

- “BuildScript.cs”

- “DeployScript.cs”

- "DeploymentScript.cs"

- "_Build/Build.cs"

- "_Build/BuildScript.cs"

- "Build/Build.cs"

- "Build/BuildScript.cs"

- "_BuildScript/BuildScript.cs"

- "_BuildScripts/BuildScript.cs"

- “BuildScript/BuildScript.cs”

- “buildscript/deployscript.cs”

- “buildscripts/buildscript.cs”

- “buildscripts/deployscript.cs”

- "BuildScript/DeploymentScript.cs"

- "BuildScripts/DeploymentScript.cs"

[Getting started section]: getting-started.md