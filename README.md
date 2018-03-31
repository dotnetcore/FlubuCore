# README #

### Flubu - Fluent builder ###

Flubu is A C# library for building projects and executing deployment scripts using C# code.

You can find FlubuCore on nuget.
* .NET Core - search for Dotnet-Flubu' cli tool
* .NET - search for 'Flubu.Runner'.


### Flubu main features / advantages ###

* Net Core support (works also on linux and macOS).
* Easy to learn and to use because you write build script entirely in C#.
* Quite a lot of built in tasks (compile, running tests, managing iis, creating deploy package, publishing nuget packages, executing powershell scripts...)
* Write your own custom c# code in script and execute it. 
* Run any external program in script.
* Reference any .net library or c# source code file in buildscript.
* Fluent interface and intelisense.
* Write tests, debug your build script.
* Use flubu tasks in any  other application.
* Web api is available for flubu. Useful for automated deployments remotely.
* Write your own flubu tasks and extend flubu fluent interface with them.

### Documentation ###

* See wiki on github

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
| Flubu | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore)](http://lucidlynx.comtrade.com:8080/job/FlubuCore) | &nbsp;
| Flubu runner System tests - tests on .net 461 mvc project | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore.Runner.SystemTests)](http://lucidlynx.comtrade.com:8080/job/FlubuCore.Runner.SystemTests) | &nbsp;
| Flubu cli tool System tests - tests on .net core 1.1 csproj project  | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore_SystemTests_Net_Core_csproj)](http://lucidlynx.comtrade.com:8080/job/FlubuCore_SystemTests_Net_Core_csproj) | &nbsp;
| Flubu cli tool System tests - tests on .net core 1.1  xproj project  | Windows| [![Build Status](http://lucidlynx.comtrade.com:8080/buildStatus/icon?job=FlubuCore_SystemTests_.Net_Core_xproj)](http://lucidlynx.comtrade.com:8080/job/FlubuCore_SystemTests_.Net_Core_xproj) | &nbsp;

FlubuCore main build and FlubuCore system tests are also runned on linux machine but status can not be displayed because build server is hosted on private server.

### Contribution guidelines ###
* If u find a bug please report it :) 
* If u have any improvement or feature proposal's we would be glad to hear from you. Add new issue as proposal and we will discuss it with you.
* If u want to fix a bug yourself, improve or add new feature to flubu.. Fork, Pull request but first add new issue so we discuss it.


[**Release notes**](https://github.com/flubu-core/flubu.core/blob/master/FlubuCore.ProjectVersion.txt
)



