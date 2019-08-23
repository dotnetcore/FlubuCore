Here you can find list of FlubuCore built in tasks with short description. Each task have fluent interface. See code documentation for detailed task documentation. If u have any questions about a task join us on gitter.

### **Tasks** 

| Task name | Description |
| --- | ----------- |
|RunProgramTask|Task runs specified external program|
|CompileSolutionTask|Task compiles specified solution.|
|PackageTask|Task Packages specified directories and files into specified directory or zip file..|
|FetchBuildVersionFromFileTask|Task fetches build version from file.|
|GitVersionTask| GitVersion is a tool to help you achieve Semantic Versioning on your project. [[ Documentation |https://gitversion.readthedocs.io/en/latest/]]
|FetchVersionFromExternalSourceTask|Task fetches build version from external source(Appveyor, travis, jenkins...).|
|GenerateCommonAssemlbyInfoTask|Task generates common assembly info for solution|
|NUnitTask|Task runs nunit tests with nunit.exe runner|
|XunitTask|Task runs xunit tests with xunit.exe runner|
|NUnitWithDotCoverTaskTask | runs nunit tests in combination with dotCover test coverage analysis|
|PublishNugetPackageTask|Task publishes nuget package to specified nuget server |
|NugetCmdLineTask| Manipulate nugets with nuget.exe  |
|UpdateXmlFileTask|Updates an XML file using the specified update commands. |
|UpdateJsonFileTask|Task updates an JSON file using the specified update commands.|
|CleanoutputTask|Task clean all projects outputs in solution.|
|ControlServiceTask|Control windows service with sc.exe command.|
|CreateWindowsServiceTask|Creates windows service with sc.exe command.|
|ExecutePowerShellScriptTask|Executes specified power shell script.|
|SqlCmdTask| Execute SQL script files with sqlcmd.exe |
|CreateAppPoolTask|Task creates new application pool in iis.|
|ControlAppPoolTask|Task can start/stop application pool.|
|DeleteAppPoolTask|Task deletes specified application pool.|
|CreateWebApplicationTask|Task creates new web application for specified web site in iis.|
|CreateWebSiteTask|Task creates new web site in iis.|
|AddWebSiteBindingTask|Task compiles specified solution.|
|GetLocalIisVersionTask|Task gets the version on iis on local machine.|
|ReplaceTokenTask|Task Replaces specified tokens in file.|
|ReplaceTextTask|Task Replaces specified texts in file.|
|CopyDirectoryStructureTask|Task copies files from one directory to another with specified filters. |
|CopyFileTask|Task copies file from from one directory to another.|
|CopyDirectoryStructureTask| Copies a directory tree from the source to the destination..|
|CreateDirectoryTask|Task creates directory at specified location.|
|DeleteDirectoryTask|Task deletes specified directory.|
|DeleteFilesTask|Task delete files from specified directory matching specified pattern.|
|UnzipFileTask|Task unzips specified zip file to specified location.|
|ZipFileTask|Task zips specified files.|
|OpenCoverTask|Task runs open cover tool.|
|OpenCoverToCoberturaTask|Task runs open cover to cobertuta tool.|
|CoverageReportTask|Task runs the coverage report generator tool.|
|LoadSolutionTask|Task load's solution information to the flubu session.|
|T4TemplateTask|Generate T4 template with TextTransform.exe utility..|
|GitTasks|Git Clone, Add, Commit, Pull, Push, Tag, RemoveFiles tasks.|
|DockerTasks|Build, Run, Stop Remove Container, Remove Image and all other tasks for coresponding docker cli commands. All tasks are genereated from offical docker documentation. |
|FlubuWebApiTasks|Various flubu web api client tasks.|

### **.net core Tasks** 
| Task name | Description |
| --- | ----------- |
|ExecuteDotnetTask|Executes specified dotnet command.
|DotnetRestoreTask|Restores the dependencies and tools for a given application / project..
|DotnetPublishTask|compiles the application, reads through its dependencies specified in the project file and publishes the resulting set of files to a directory.
|DotnetBuildTask|Builds a project and all of its dependencies.
|DotnetPackTask|command builds the project and creates NuGet packages. The result of this command is a NuGet package.
|DotnetNugetPushTask|Pushes the nuget package to the nuget server.
|DotnetTestTask|Runs tests using a test runner specified in the project.json / csproj.
|DotnetCleanTask| Cleans the output of a project.
|DotnetToolTask| All dotnet tool commands.
|DotnetEfTasks| Various entity framework tasks.
|UpdateNetCoreVersionTask| Updates the version in csproj / project.json file
|CoverletTask| Coverlet is a cross platform code coverage library for .NET Core, with support for line, branch and method coverage [[ Documentation |https://github.com/tonerdo/coverlet]].
|SshComandLinuxTask| Runs specified command on the remote host.
|SshCopyLinuxTask|Copy projects/files to the remote host.
|SystemCtlLinuxTask|Runs system ctl.

