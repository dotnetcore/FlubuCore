rmdir %userprofile%\.nuget\packages\dotnet-flubu /S /Q
rmdir %userprofile%\.nuget\packages\flubu.core /S /Q
rmdir %userprofile%\.nuget\packages\flubu /S /Q
rmdir %userprofile%\.nuget\packages\.tools\dotnet-flubu /S /Q

dotnet restore Flubu
dotnet restore dotnet-flubu
dotnet restore Flubu.Tests

dotnet pack Flubu -c Release --version-suffix %1
dotnet pack dotnet-flubu -c Release --version-suffix %1

dotnet test Flubu.Tests -xml results.xml
