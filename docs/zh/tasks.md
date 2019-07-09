本页面罗列了 FlubuCore 的内置任务及其简单描述。每个任务都支持流畅接口（fluent interface），更详细的任务文档请查阅代码文档。如果你有任何疑问，请加入我们的 gitter。

### **任务**

| 名称                               | 描述                                                                                                                          |
| ---------------------------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| RunProgramTask                     | 运行指定的外部程序                                                                                                            |
| CompileSolutionTask                | 编译指定的解决方案                                                                                                            |
| PackageTask                        | 将指定的目录和文件打包（package）到特定的目录或 zip 文件中                                                                    |
| FetchBuildVersionFromFileTask      | 从文件中获取构建版本（build version）                                                                                         |
| GitVersionTask                     | GitVersion 工具可帮助你在项目中实现语义版本控制（Semantic Versioning）[更多...](https://gitversion.readthedocs.io/en/latest/) |
| FetchVersionFromExternalSourceTask | 从外部源（Appveyor、travis、jenkins 等）获取构建版本（build version）                                                         |
| GenerateCommonAssemlbyInfoTask     | 为解决方案生成通用程序集信息（common assembly info）                                                                          |
| NUnitTask                          | 使用 nunit.exe runner 运行 nunit 测试                                                                                         |
| XunitTask                          | 使用 xunit.exe runner 运行 xunit 测试                                                                                         |
| NUnitWithDotCoverTaskTask          | 结合 dotCover 测试覆盖率分析（test coverage analysis）运行 nunit 测试                                                         |
| PublishNugetPackageTask            | 将 nuget 包发布到指定的 nuget 服务器上                                                                                        |
| NugetCmdLineTask                   | 通过 nuget.exe 操作 nuget                                                                                                     |
| UpdateXmlFileTask                  | 通过指定的更新命令更新 XML 文件                                                                                               |
| UpdateJsonFileTask                 | 通过指定的更新命令更新 JSON 文件                                                                                              |
| CleanoutputTask                    | 清理解决方案中所有项目的输出                                                                                                  |
| ControlServiceTask                 | 通过 sc.exe 命令控制 Windows 服务                                                                                             |
| CreateWindowsServiceTask           | 通过 sc.exe 命令创建 Winding 服务                                                                                             |
| ExecutePowerShellScriptTask        | 执行指定的 PowerShell 脚本                                                                                                    |
| SqlCmdTask                         | 通过 sqlcmd.exe 执行指定的 SQL 脚本文件                                                                                       |
| CreateAppPoolTask                  | 在 IIS 中创建新的应用程序池（application pool）                                                                               |
| ControlAppPoolTask                 | 启动/停止应用程序池                                                                                                           |
| DeleteAppPoolTask                  | 删除特定的应用程序池                                                                                                          |
| CreateWebApplicationTask           | 在 IIS 中为指定的 Web 站点创建新 Web 应用程序（web application）                                                              |
| CreateWebSiteTask                  | 在 IIS 中创建新站点（web site）                                                                                               |
| AddWebSiteBindingTask              | 编译（compile）指定的解决方案                                                                                                 |
| GetLocalIisVersionTask             | 获取本地机器上 IIS 的版本                                                                                                     |
| ReplaceTokenTask                   | 替换文件中指定的标记（token）                                                                                                 |
| ReplaceTextTask                    | 替换文件中指定的文本（text）                                                                                                  |
| CopyDirectoryStructureTask         | 使用指定的过滤器（filter）将文件从一个目录（directory）复制到另一个目录                                                       |
| CopyFileTask                       | 将文件从一个目录（directory）复制到另一个目录                                                                                 |
| CopyDirectoryStructureTask         | 将目录树（directory tree）从一处复制到另一处（from the source to the destination)                                             |
| CreateDirectoryTask                | 在指定路径下创建目录                                                                                                          |
| DeleteDirectoryTask                | 删除指定的目录                                                                                                                |
| DeleteFilesTask                    | 根据指定的模式（pattern）在指定的目录下删除文件                                                                               |
| UnzipFileTask                      | 将 zip 文件解压缩到指定的路径下                                                                                               |
| ZipFileTask                        | 压缩指定的数个文件                                                                                                            |
| OpenCoverTask                      | 运行 OpenCover                                                                                                                |
| OpenCoverToCoberturaTask           | 运行 OpenCover 至 Cobertuta                                                                                                   |
| CoverageReportTask                 | 运行覆盖率报告生成器                                                                                                          |
| LoadSolutionTask                   | 在 flubu 会话中加载解决方案信息                                                                                               |
| T4TemplateTask                     | 使用 TextTransform.exe 工具生成 T4 模板                                                                                       |
| GitTasks                           | 围绕 Git 克隆、添加、提交、拉取、推送、标签和移除文件的任务                                                                   |
| DockerTasks                        | 构建、运行、停止与移除容器，移除镜像以及其它相应的 docker cli 命令。所有任务均来自 docker 官方文档。                          |
| FlubuWebApiTasks                   | 各种 flubu web api 客户端任务                                                                                                 |

### **.NET Core 任务**

| 名称                     | 描述                                                                                                                                          |
| ------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------- |
| ExecuteDotnetTask        | 执行指定的 dotnet 命令                                                                                                                        |
| DotnetRestoreTask        | 为指定应用程序或项目恢复（restore）依赖项（dependencies）和工具（tools）                                                                      |
| DotnetPublishTask        | 编译应用程序，读取项目文件中指定的依赖项，并将生成的文件集（set of files）发布到目录（directory）中                                           |
| DotnetBuildTask          | 构建（build）项目及其所有依赖项                                                                                                               |
| DotnetPackTask           | 通过命令构建项目并创建 nuget 包，该命令将生成一个 nuget 包                                                                                    |
| DotnetNugetPushTask      | 推送 nuget 包至 nuget 服务器                                                                                                                  |
| DotnetTestTask           | 根据 project.json / csproj 所配置的 test runner，运行测试                                                                                     |
| DotnetCleanTask          | 清理项目输出                                                                                                                                  |
| DotnetEfTasks            | 包含了多种实体框架（entity framework）任务                                                                                                    |
| UpdateNetCoreVersionTask | 在 csproj / project.json 文件中更新版本                                                                                                       |
| CoverletTask             | Coverlet 是 .NET Cire 下跨平台代码覆盖库，支持行（lines）、分支（branch）和方法（methods）覆盖 [更多...](https://github.com/tonerdo/coverlet) |
| SshComandLinuxTask       | 向远程主机运行指定的命令                                                                                                                      |
| SshCopyLinuxTask         | 将项目或文件复制到远程主机                                                                                                                    |
| SystemCtlLinuxTask       | 运行 systemctl 命令                                                                                                                           |
