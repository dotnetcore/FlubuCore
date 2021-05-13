## **Referencing external assemblies in build script**

FlubuCore loads all assemblies references and nuget packages automatically from build script csproj file. Csproj must be at on of the location specified [here](https://github.com/dotnetcore/FlubuCore/blob/develop/src/FlubuCore/Scripting/Analysis/ProjectFileAnalyzer.cs) or the path to the csproj file must be specified in .flubu file.  If not assembly and nuget references will not be loaded automatically when executing script.

!!! Note
	You can also disable referencing assemblies and nuget packages from build script by adding attribute to build script.

	```C#
	[DisableLoadScriptReferencesAutomatically]
	public class BuildScript : DefaultBuildScript
	{
	}
	```

Alternatively when you are running scripts without csproj(for example deploy scripts) external references can be added  with directives in three ways:

<a name="By-assembly-relative-or-full-path"></a>

### **By assembly relative or full path**

On the build script class you have to add attribute:

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
FlubuCore can also load all assemblies from specified directory and optionaly from it's subdirectories

```C#
[AssemblyFromDirectory(@".\Packages", true)]
public class BuildScript : DefaultBuildScript
{
}
```

<a name="Referencing-nuget-packages"></a>

### **Referencing nuget packages**

Flubu supports referencing nuget packages. .net core sdk or msbuild must be installed if u want to reference nuget packages otherwise they will not get restored.

You have to add NugetPackage attribute on the script class:

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

### **Load assembly by assembly full name**

System assemblies can be loaded by fully qualifed assemlby name.

You have to add Reference attribute on the script class:

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

One way to get fully qualifed assembly name:

    var fullQualifedAssemblyName = typeof(XmlDocument).Assembly.FullName;

<a name="Load-all-assemblies-from-directory"></a>

### **Load all assemblies from directory**
Even if you are not using your script together with csproj flubu can load all external assemblies for you automatically from directory (assemblies in subdirectories are also loaded ). 

By default flubu loads all assemblies from directory FlubuLib. Just create the directory at the flubu runner location and put assemblies in that directory. You can specify directory in flubu runner from where to load assemblyes also:

`flubu.exe -ass=somedirectory`

`dotnet flubu -ass=somedirectory`
alternatively you can put ass key into flubusettings.json file:

    {
      "ass" : "someDirectory",
      "SomeOtherKey" : "SomeOtherValue"
    }` 

<a name="Adding-other-cs-files-to-build-script"></a>

## **Adding other .cs files to script**

Other .cs files have to be added through attribute they are not automatically loaded from buildscript project file.
Exception are build script base classes and partial classes they are loaded automatically.

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

FlubuCore can also load all .cs files to script from specified directory and optionaly from it's subfolders.

```C#
[IncludeFromDirectory(@".\Helpers", true)]
public class BuildScript : DefaultBuildScript
{
}
```