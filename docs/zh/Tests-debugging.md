### **Debug script through .net core console application**

The easiest way to debug your script is that you place your script in a console application and execute it in console application. After that you can debug your script as any other application.

How to execute scripts through console application can be found [here.](/execute-script-in-console-app-or-pack-as-global-tool/)

### **编写测试构建脚本与通过测试调试构建脚本**

Wiki 文档即将推出。先通过简单的测试来上手：https://github.com/dotnetcore/FlubuCore.Examples/blob/master/NetCore_csproj/BuildScript/BuildScriptTests.cs

如有需要，你可以通过测试调试构建脚本。

你也可以在其它 .NET 应用程序中使用 flub 任务，就像上面的测试示例一样。

### **通过附加到运行中的进程来调试构建脚本**

你可以通过调试器（debugger）附加到 Flubu 进程来调试构建脚本。

- 由于 Flubu 会稍微改变构建脚本，所以你必须将 Visual Studio 的「要求源文件与原始版本完全匹配」的选项禁用掉。
  这个选项的位置在「工具」→「选项」→「调试」→「常规」→「要求源文件与原始版本完全匹配」。对于 VS Code 来说则不能确定。
- 建议在第一个断点（break point）之前，在 ITaskContext 上使用 WaitForDebugger 扩展方法：

        protected override void ConfigureTargets(ITaskContext context)
        {
            context.WaitForDebugger();
        }

- 运行构建脚本，并将调试器（debugger）附加到 FlubuCore 进程。FlubuCore 进程名取决于你所使用 FlubuCore Runner。
  - FlubuCore.Runner - 你需要将调试器附加到名为 flubu.exe 的进程
  - dotnet-flubu Cli 工具 - 你需要将调试器附加到名为 dotnet 的确切的进程中
  - FlubuCore.GlobalTool - 你需要将调试器附加到名为 Flubu 的进程中
