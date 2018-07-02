# README #

### FlubuCore - Fluent builder ###

FlubuCore is A C# library for building projects and executing deployment scripts using C# code.

See [**getting started**](https://github.com/flubu-core/flubu.core/wiki/1-Getting-started) section to get you started. [**build script fundamentals**](https://github.com/flubu-core/flubu.core/wiki/2-Build-script-fundamentals)
list of features that FlubuCore has to offer and describes how to use them. Alternatively You can take a look at examples below.

You can find FlubuCore on nuget:
* .NET - [**FlubuCore.Runner**](https://www.nuget.org/packages/FlubuCore.Runner/)
* .NET Core CLI tool - [**dotnet-flubu**](https://www.nuget.org/packages/dotnet-flubu/)
* .NET Core global tool - ```dotnet tool install --global FlubuCore.GlobalTool```

### FlubuCore main features / advantages ###

* Net Core support.
* Easy to learn and to use because you write build script entirely in C#.
* Quite a lot of built in tasks (compile, running tests, managing iis, creating deploy package, publishing nuget packages, executing powershell scripts...)
* Write your own custom c# code in script and execute it. 
* Run any external program in script.
* Reference any .net library or c# source code file in buildscript. Now also available option to reference nuget package in build script.
* Fluent interface and intelisense.
* Write tests, debug your build script.
* Use flubu tasks in any  other application.
* Web api is available for flubu. Useful for automated deployments remotely.
* Write your own flubu tasks and extend flubu fluent interface with them.

### Have a question? ###

 [![Join the chat at https://gitter.im/FlubuCore/Lobby](https://badges.gitter.im/mbdavid/LiteDB.svg)](https://gitter.im/FlubuCore/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

### Examples ###
* [**.net simple example**](https://github.com/flubu-core/examples/blob/master/MVC_NET4.61/BuildScripts/BuildScriptSimple.cs
)

* [**.net example**](https://github.com/flubu-core/examples/blob/master/MVC_NET4.61/BuildScripts/BuildScript.cs
)

* [**.net core example**](https://github.com/flubu-core/examples/blob/master/NetCore_csproj/BuildScript/BuildScript.cs
)

* [**.deploy script example**](https://github.com/flubu-core/examples/blob/master/DeployScriptExample/BuildScript/DeployScript.cs
)

### Build status ###

| Job              | Platform     | Build status                                                                                                                                                        | 
|-----------------------------|--------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| FlubuCore | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore)](http://lucidlynx.comtrade.com:8080/job/FlubuCore) | &nbsp;
| FlubuCore.Runner system tests - tests on .net 461 mvc project | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore.Runner.SystemTests)](http://lucidlynx.comtrade.com:8080/job/FlubuCore.Runner.SystemTests) | &nbsp;
| FlubuCore cli tool System tests - tests on .net core 1.1 csproj project  | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore_SystemTests_Net_Core_csproj)](http://lucidlynx.comtrade.com:8080/job/FlubuCore_SystemTests_Net_Core_csproj) | &nbsp;
| FlubuCore cli tool System tests - tests on .net core 1.1  xproj project  | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore_SystemTests_.Net_Core_xproj)](http://lucidlynx.comtrade.com:8080/job/FlubuCore_SystemTests_.Net_Core_xproj) | &nbsp;
| FlubuCore.WebApi deployment tests  | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore_WebApi_DeploymentTests)](http://lucidlynx.comtrade.com:8080/job/FlubuCore_WebApi_DeploymentTests) | &nbsp;

FlubuCore main build and FlubuCore system tests are also runned on linux machine but status can not be displayed because build server is hosted on private server.

### Contribution guidelines ###
* If u find a bug please report it :) 
* If u have any improvement or feature proposal's we would be glad to hear from you. Add new issue as proposal and we will discuss it with you.
* If u want to fix a bug yourself, improve or add new feature to flubu.. Fork, Pull request but first add new issue so we discuss it.


[**Release notes**](https://github.com/flubu-core/flubu.core/blob/master/FlubuCore.ProjectVersion.txt
)



