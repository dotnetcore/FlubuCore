

In .net you run flubu build scripts with console application build.exe. Add Flubu.Runner nuget to get the console application.

In .net core you run flubu build scripts with dotnet command line tool or global tool. Add FlubuCore nuget. You also have to add dotnet-flubu as dotnet cli tool reference to project. How to add it as dotnet cli tool see getting started section.

### **Usage**


.net core usage: `dotnet flubu {TargetToExecute} {Flubu options} {build script arguments}`

.net core global tool usage: `flubu {TargetToExecute} {Flubu options} {build script arguments}`

.net usage: flubu.exe `{TargetToExecute} {Flubu options} {build script arguments}`

#### Multiple target execution

.net core usage: dotnet flubu {TargetToExecute} {TargetToExecute2} {TargetToExecute3...} {Options}

.net usage: build.exe {TargetToExecute} {TargetToExecute2} {TargetToExecute3...} {Options}

### **Flubu options**

![N/A](img/FlubuHelp.png "Flubu help")

### **target help**

List all available targets:

`build.exe help`

`dotnet flubu help`

Target specific help:

`build.exe {TargetName} help`

`dotnet flubu {TargetName} help`

It displays all tasks with description that will be executed by target. It also displays which argument(with description) can be passed through to specific task in target. 

### **Examples**

`flubu.exe compile`

`dotnet flubu compile -s=Scripts/MyBuildscript.cs`

`dotnet flubu --help`

`flubu compile`

### **Default build script locations**

- “BuildScript.cs”

- “DeployScript.cs”

- “BuildScript/BuildScript.cs”

- “buildscript/deployscript.cs”

- “buildscripts/buildscript.cs”

- “buildscripts/deployscript.cs”