## **在构建脚本中引用外部程序集**

FlubuCore 会自动从构建脚本的 csproj 文件中加载所有程序集引用（assemblies references）和 nuget 包。csproj 文件必须位于[指定位置](https://github.com/dotnetcore/FlubuCore/blob/develop/src/FlubuCore/Scripting/Analysis/ProjectFileAnalyzer.cs)。如果不是程序集引用和 nuget 引用的话，FlubuCore 就不会在执行时自动加载它们。

!!! Note
	你可以在构建脚本中通过添加特性（attribute）的方式禁用引用程序集和 nuget 包。

	```C#
	[DisableLoadScriptReferencesAutomatically]
	public class BuildScript : DefaultBuildScript
	{
	}
	```

或者，当你运行无 csproj 的脚本（比如部署脚本）时，外部引用（external references）可以通过三种方式用指令来添加：

<a name="By-assembly-relative-or-full-path"></a>

### **通过程序集的相对路径或完整路径**

在构建脚本类上，你得添加特性（attribute）：

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

FlubuCore 还可以从指定的目录价在所有程序集，也可以从其子目录（subdirectories）中加载

```C#
[AssemblyFromDirectory(@".\Packages", true)]
public class BuildScript : DefaultBuildScript
{
}
```

<a name="Referencing-nuget-packages"></a>

### **引用 NuGet 包**

Flubu 支持引用 NuGet 包。如果你想引用 NuGet 包，你必须先安装 .NET Core SDK 或 msbuild，否则它们（NuGet 包）将无法还原（restore）。

你必须在脚本的类上添加 NuGetPackage 特性：

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

### **按程序集名称加载**

FlubuCore 可以通过完全限定程序集名（fully qualifed assemlby name）的方式加载系统程序集（system assemblies）。

逆序比在脚本类上添加 Reference 特性（attribute）：

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

获取完全限定程序集名（fully qualifed assembly name）的一种方式：

    var fullQualifedAssemblyName = typeof(XmlDocument).Assembly.FullName;

<a name="Load-all-assemblies-from-directory"></a>

### **从目录中加载所有程序集**

即便你没有将脚本与 csproj 一道使用，FlubuCore 也可以从目录中自动加载所有外部程序集（external assemblies）（子目录（subdirectories）中的程序集也可以选择一并被加载）。

默认情况下，FlubuCore 会从 FlubuLib 目录下加载所有程序集。只要在 flubu runner 所在的位置下创建目录，并将程序集刚在该目录之中。你也可以在 flubu runner 中指明从那个目录加载程序集：

`flubu.exe -ass=somedirectory`

`dotnet flubu -ass=somedirectory`

或者你可以通过给 flubusettings.json 文件的 ass 设置一个值：

    {
      "ass" : "someDirectory",
      "SomeOtherKey" : "SomeOtherValue"
    }`

<a name="Adding-other-cs-files-to-build-script"></a>

## **将其它 .cs 文件加到脚本中**

其它 .cs 文件必须先添加特性（attribute），它们不会被自动地从构建脚本的项目文件中加载。除非已经自动加载过构建脚本的基类或部分类。

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

FlubuCore 还可以从指定的目录中加载所有的 .cs 文件到脚本中（也可以选择将该目录下的子文件夹中的 .cs 文件一并加载）。

```C#
[IncludeFromDirectory(@".\Helpers", true)]
public class BuildScript : DefaultBuildScript
{
}
```
