在 .NET 中，你可以通过控制台应用程序 flubu.exe 运行 flubu 构建脚本。可以通过 nuget 添加 Flubu.Runner 以便获取控制台应用程序。

在 .NET Core 中可以用 dotnet 命令或全局工具来运行 flubu 构建脚本。通过 nuget 添加 FlubuCore，将 dotnet-flubu 作为 dotnet cli 工具引入项目。至于如何将其添加为 dotnet cli 工具请移步**入门**一节。

### **如何使用**

.NET core 用法：`dotnet flubu {TargetToExecute} {Flubu options} {build script arguments}`

.NET core 全局工具的用法：`flubu {TargetToExecute} {Flubu options} {build script arguments}`

.NET 用法：flubu.exe `{TargetToExecute} {Flubu options} {build script arguments}`

#### 多目标执行

.NET core 用法：dotnet flubu {TargetToExecute} {TargetToExecute2} {TargetToExecute3...} {Options}

.NET 用法：flubu.exe {TargetToExecute} {TargetToExecute2} {TargetToExecute3...} {Options}

Target 可以和添加有 `-parallel` 选项的任务一同执行。

### **Flubu 选项**

![N/A](img/FlubuHelp.png "Flubu help")

### **帮助**

列出所有可用目标：

`flubu help`

`dotnet flubu help`

对特定目标的帮助：

`flubu {TargetName} help`

`dotnet flubu {TargetName} help`

这条命令将列出所有描述有执行目标的任务，以及哪些参数可以传递给目标（target）中的特定任务。

### **默认构建脚本的位置**

- “BuildScript.cs”

- “DeployScript.cs”

- “BuildScript/BuildScript.cs”

- “buildscript/deployscript.cs”

- “buildscripts/buildscript.cs”

- “buildscripts/deployscript.cs”
