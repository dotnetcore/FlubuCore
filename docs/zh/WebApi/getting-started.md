## **关于**

你可以通过使用 FlubuCore Web Api 远程执行 FlubuCore 脚本。这主要是为了让 .NET 或 .NET Core 应用程序从构建服务器（build server）部署到不同环境的自动化部署可以通过任意 FLubuCore 脚本来执行。

Web Api 支持：

- 在部署有 FlubuCore Web Api 的服务器上执行 flubu 脚本；
- 在部署有 FlubuCore Web Api 的服务器上上传（发布或其它操作）包（package）；
- 在部署有 FlubuCore Web Api 的服务器上删除包；
- 向客户端发送报告；
- 在部署有 FlubuCore Web Api 的服务器上上传 FlubuCore 脚本；
- 自动更新；
- 通过 FlubuCore Web App（与 Web Api 一同部署）手动执行目标（target）。

本指南将包含以下内容：

- 将 FlubuCore Web Api 部署到服务器
- 编写用于部署 MVC 示例应用程序于服务器的 .NET 部署脚本。如果你想通过 FlubuCore.WebApi 部署 .NET Core 应用程序，你应当阅读一下本指南。使用 FlubuCore.WebApi 部署 .NET Core 或 .NET 应用程序是没什么很大的差别的，所有差异都会写在本指南中。
- 编写构建脚本，将示例应用程序的部署包（deployment package）上传到服务器，并执行我们编写的部署脚本。
- 通过构建脚本（build script）远程运行部署脚本（deployment script）。

<a name="requirements"></a>

### **要求**

- .NET 运行时 4.6.2 或更高，或在服务器上安装 .NET Core Runtime，这取决于你打算使用的 FlubuCore.WebApi 构建哪一种应用。

<a name="Web-api-deployment"></a>

### **Web Api 开发**

- 从 https://github.com/dotnetcore/FlubuCore/releases 获取适当的 Web Api 部署包；
- 将 Web Api 部署包复制到需要执行 flubu 脚本的服务器上；
- 解压缩包；
- 在解压缩的 DeploymentConfig.json 文件中设置 Web Api 部署配置设置。有关特定部署设置的更多信息可见配置文件；
- 在 Windows 服务器上运行 deploy.bat 部署 Web Api；
- 在 Linux/macOS 服务器上依次运行 `dotnet restore`、`dotnet flubu -s=deploymentscript.cs`
- 在部署位置运行 `dotnet FlubuCore.WebApi.dll`，启动自托管（selfhost）的 Web Api。你当然也可以在 IIS 上托管它……

#### IIS 开发

如何部署 ASP.NET Core 应用程序，请查阅：https://docs.microsoft.com/en-us/aspnet/core/publishing/iis?tabs=aspnetcore2x

某些操作可能需要管理员权限（administration rights），比如启动/停止应用程序池（application pool）。如果是这种情况，你必须在托管 Web Api 的应用程序池中修改身份。切换到「应用程序池」→「Web Api 应用程序池」→「高级设置...」→「进程模型」→「标识」，并更改为具有管理员权限的账户。

<a name="Write-deploy-script"></a>

### **编写部署脚本**

用于示例的 .NET 部署脚本可以在[这里](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs)查看。如果你想尝试该示例，最好的方法是复制 flubu core 的目录。.NET Core 应用程序的部署脚本会略有不同。

.NET 应用程序的示例部署脚本将：

- 如果不存在，创建 IIS 应用程序池；
- 停止应用程序池；
- 从 /packages 目录下解压缩包（将使用构建脚本上传到 Web Api）；
- 复制解压缩的应用程序到托管它的新文件夹；
- 在 IIS 上创建站点，如 Web 应用程序；
- 启动应用程序池。

完成部署脚本的编写之后，手动将其复制到 Web Api 部署位置的 /scripts 文件夹下。Web Api 也可以上传脚本，但出于安全考虑，这个功能默认是禁用的。在大多数情况下应保持禁用状态。

如果需要，你可以根据具体情况修改示例 DeployScript。

<a name="Write-build-script"></a>

### **编写构建脚本**

用于示例的 .NET 构建脚本可以在[这里](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/BuildScript.cs)查看。

.NET 示例构建脚本将：

- 获取身份验证令牌（authentication token）
- 在 Web Api 上删除 /packages 目录下的旧的包
- 上传包（package）到 Web Api 的 /packages 文件夹下
- 执行 /scripts 文件夹下我们手动上传的部署脚本

如果需要，你可以根据具体情况修改示例 BuildScript。

<a name="Run-deploy-script"></a>

### **运行部署脚本**

如果你克隆（clone）了示例代码仓库，只需要在根目录（DeployScriptExample folder）下执行：

- `dotnet restore buildscript.csproj`，以及
- `dotnet flubu deploy -s=buildscript\buildscript.cs`

实际上你可能在成功构建之后从构建服务器部署。在合并到发布分支之后，在构建服务器上手动执行作业（job）……

<a name="Security"></a>

### **安全**

如果攻击者能获取 Web Api 的访问权限，那么他可能会进行大量破坏（do alot of damage）。如有可能，应实施以下安全举措（security measures）：

- 如有可能，FLubu Web Api 不应具有对外公开访问的权限；
- 始终通过 https 托管 Web Api；
- 通过 ip（通过配置）限制访问；
- 通过时间范围（通过配置）限制访问。如果你的应用程序在固定时间（如晚上 11 点）部署则应采用本安全策略，然后设置文档 api 的时间范围（如晚上 11 时至 11 时 15 分）；
- 使用强密码（Web Api 用户创建）
- 不要禁用「在登录失败时限制访问」的功能（通过配置）；
- 在 api 上执行 GetToken/Script 时启用电子邮件通知（通过配置）。

关于安全设置（security settings）的详细说明，请参阅 Web Api 的 appsettings.json 文件。

<a name="Automatic-update"></a>

### **自动更新**

如果存在可用的新版本，你可以自动更新 FlubuCore Web Api。只需要导航到 /UpdateCenter（不是 /api/UpdateCenter）

<a name="manual-target-execution"></a>

### **通过 FlubuCore Web App 远程手动执行目标**

你可以通过 FlubuCore Web App 手工执行目标（target），只需要导航到 /Script
