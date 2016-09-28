rmdir %userprofile%\.nuget\packages\dotnet-flubu /S /Q
rmdir %userprofile%\.nuget\packages\flubu.core /S /Q
rmdir %userprofile%\.nuget\packages\flubu /S /Q
rmdir %userprofile%\.nuget\packages\FlubuCore /S /Q
rmdir %userprofile%\.nuget\packages\.tools\dotnet-flubu /S /Q

dotnet restore FlubuCore
dotnet restore dotnet-flubu
dotnet restore Flubu.Tests

dotnet pack FlubuCore -c Release
dotnet pack dotnet-flubu -c Release

dotnet test Flubu.Tests -xml results.xml
