using System;
using System.Collections.Generic;
using System.Text;
using FlubuCore.Tasks.Git;
using FlubuCore.Tasks.MsSql;
using FlubuCore.Tasks.NetCore;
using FlubuCore.Tasks.Testing;
using FlubuCore.Tasks.Versioning;

namespace FlubuCore.Infrastructure.Terminal.Commands
{
    public static class DotnetCommands
    {
        public static List<Hint> Commands { get; } = new List<Hint>
        {
            new Hint { Name = "dotnet build", Help = "The dotnet build command builds the project and its dependencies into a set of binaries.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet test", Help = "The dotnet test command is used to execute unit tests in a given project.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet pack", Help = "The dotnet pack command builds the project and creates NuGet packages.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet publish", Help = "Packs the application and its dependencies into a folder for deployment to a hosting system.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet restore", Help = "The dotnet restore command uses NuGet to restore dependencies as well as project-specific tools that are specified in the project file.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet nuget push", Help = "The dotnet nuget push command pushes a package to the server and publishes it.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet clean", Help = "The dotnet clean command cleans the output of the previous build.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet msbuild", Help = "Builds the specified targets in the project file.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet tool install", Help = "The dotnet tool install command provides a way for you to install .NET Core Global Tools on your machine. ", OnlySimpleSearh = true },
            new Hint { Name = "dotnet tool uninstall", Help = "Uninstalls the specified .NET Core Global Tool from your machine.", OnlySimpleSearh = true },
            new Hint { Name = "dotnet tool update", Help = "Updates the specified .NET Core Global Tool on your machine.", OnlySimpleSearh = true },
        };

        public static Dictionary<string, Type> SupportedExternalProcesses { get; } = new Dictionary<string, Type>()
        {
            { "dotnet build", typeof(DotnetBuildTask) },
            { "dotnet clean", typeof(DotnetCleanTask) },
            { "dotnet pack", typeof(DotnetPackTask) },
            { "dotnet publish", typeof(DotnetPublishTask) },
            { "dotnet test", typeof(DotnetTestTask) },
            { "dotnet restore", typeof(DotnetRestoreTask) },
            { "dotnet nuget push", typeof(DotnetNugetPushTask) },
            { "dotnet msbuild", typeof(DotnetMsBuildTask) },
            { "dotnet tool install", typeof(DotnetToolInstall) },
            { "dotnet tool uninstall", typeof(DotnetToolUninstall) },
            { "dotnet tool update", typeof(DotnetToolUpdate) },
            { "gitversion", typeof(GitVersionTask) },
            { "sqlcmd", typeof(SqlCmdTask) },
            { "coverlet", typeof(CoverletTask) },
        };
    }
}