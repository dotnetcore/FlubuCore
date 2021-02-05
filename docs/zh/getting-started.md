# FlubuCore 入门

## .NET core 篇

---

<a name="Requirements-.net-core"></a>

<a name="Installation-.net-core"></a>

#### 安装

- 添加新项目到解决方案，比如：BuildScript；
- 在项目中用 NuGet 添加对 **[FlubuCore]** 的引用。

##### 或从 FlubuCore 模板安装

- 在控制台中将当前目录切换到解决方案所在之处。
- 运行 `dotnet new -i FlubuCore.Template`
- 运行 `dotnet new FlubuCore`
- 这一步将从 FlubuCore 引用和 FlubuCore 脚本模板中添加 BuildScript.csproj

<a name="Write-and-run-your-first-build-script-in-.net-core"></a>

#### 用 .NET Core 编写并运行第一个构建脚本

将 BuildScript.cs 文件添加到构建脚本项目之中，并添加以下代码。

以下代码将编译目标（compile target）添加到 flubu 命令中，编译目标将编译解决方案。

```C#
public class MyBuildScript : DefaultBuildScript
{
    protected override void ConfigureTargets(ITaskContext context)
    {
        var compile = context.CreateTarget("compile")
			.SetDescription("Compiles the solution.")
            .AddCoreTask(x => x.Build("FlubuExample.sln"));
    }
}
```

<a name="Run-build-script-Core"></a>

#### 用 dotnet CLI 工具运行构建脚本

- 将 [dotnet-flubu] 作为 dotnet 工具添加到 csproj 文件或 xproj（project.json）文件中。如果你是从模板安装的 FlubuCore，则不需要这个步骤。

csproj:

```xml
<ItemGroup>
    <DotNetCliToolReference Include="dotnet-flubu" Version="1.7.0" />
</ItemGroup>
```

project.json:

```json
"tools": {
        "dotnet-flubu": {
            "version": "1.7.0"
        }
    }
```

- 运行 `dotnet restore`。这条命令将还原（restore） dotnet-flubu 包，并将其添加到 dotnet 工具命令中。

- 运行 `dotnet flubu help`。除了默认命令外，你还能看到刚才我们添加进去的编译命令。

- 运行 `dotnet flubu compile`，这条命令将编译你的解决方案。

<a name="Run-build-script-core-with-global-tool"></a>

#### 在全局工具中运行构建脚本

- 必须先安装 .net core sdk 2.1.300 或更高版本；
- 安装 FlubuCore 全局工具：`dotnet tool install --global FlubuCore.Tool`；
- 在构建脚本所在的目录下运行 `flubu compile`，本命令将编译你的解决方案。

这是个非常基础的构建脚本，目的是帮你快速入门。FlubuCore 已为你提供了许多棒极了的功能。你可以到 [Build script fundamentals] 阅读更多，或者移步 [.net core examples] 查看大多数主要功能的用法。同时建议你查看 [FlubuCore 交互模式](build-script-runner-interactive.md)和[覆盖现有选项或通过控制台向任务添加其他选项](override-add-options.md)一节。


## .NET 篇

---

#### 要求

引用 FlubuCore 的构建脚本项目必须是 .NET Framework 4.6.2 或更高。如果这一点无法做到，那么可以使用低于 [Flubu] 2.64 的版本，只要你安装了 .NET Framework 4.0 环境。为了运行 FlubuCore Runner，你需要安装 .NET 运行时 4.0 或更高的版本。

<a name="Installation.net"></a>

#### 安装

- 添加新项目到解决方案，比如：BuildScript；
- 在项目中用 NuGet 添加对 **[FlubuCore.Runner]** 的引用。这一步将会引用 FlubuCore.dll 并会将 BuildScript.cs 文件（构建脚本模板）添加到项目之中，为运行脚本添加 flubu.exe。

<a name="write-and-run"></a>

#### 用 .NET 编写并运行第一个构建脚本

使用以下代码修改 BuildScript.cs 文件，将解决方案的名字换成你的。

以下代码将编译目标（compile target）添加到 flubu 密令中，编译目标将编译解决方案。

```C#
using FlubuCore.Context;
using FlubuCore.Scripting;

public class BuildScript : DefaultBuildScript
{
	protected override void ConfigureBuildProperties(IBuildPropertiesContext context)
    {
		context.Properties.Set(BuildProps.ProductId, "FlubuExample");
        context.Properties.Set(BuildProps.ProductName, "FlubuExample");
        context.Properties.Set(BuildProps.SolutionFileName, "FlubuExample.sln");
        context.Properties.Set(BuildProps.BuildConfiguration, "Release");
    }

    protected override void ConfigureTargets(ITaskContext session)
    {
        var compile = session.CreateTarget("compile")
         .SetDescription("Compiles the solution.")
         .AddTask(x => x.CompileSolutionTask());
    }
}
```

<a name="run-build-script"></a>

#### 运行构建脚本

- 打开控制台程序（cmd）并将当前目录导航到构建脚本项目所在之处；

- 运行 flubu.exe help。除了默认命令外，你还能看到刚才我们添加进去的编译命令。

- 运行 flubu.exe compile，该命令将编译你的解决方案。

这是个非常基础的构建脚本，目的是帮你快速入门。FlubuCore 已为你提供了许多棒极了的功能。你可以到 [Build script fundamentals] 阅读更多，或者移步 [.net examples] 查看大多数主要功能的用法。

<a name="Getting-started-.net-core"></a>

[csproj.png]: https://bitbucket.org/repo/Bnjqgy/images/3977856142-csproj.png
[projectjson.png]: https://bitbucket.org/repo/Bnjqgy/images/2485583270-projectjson.png
[flubu examples]: https://github.com/flubu-core/examples
[build script fundamentals]: buildscript-fundamentals.md
[.net examples]: https://github.com/flubu-core/examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs
[.net core examples]: https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs
[flubu]: https://www.nuget.org/packages/Flubu
[flubucore]: https://www.nuget.org/packages/FlubuCore
[flubucore.runner]: https://www.nuget.org/packages/FlubuCore.Runner/
[dotnet-flubu]: https://www.nuget.org/packages/dotnet-flubu/
