rmdir %userprofile%\.nuget\packages\dotnet-flubu /S /Q
rmdir %userprofile%\.nuget\packages\flubu.core /S /Q
rmdir %userprofile%\.nuget\packages\flubu /S /Q
rmdir %userprofile%\.nuget\packages\.tools\dotnet-flubu /S /Q

dotnet pack Flubu -c Release
dotnet pack dotnet-flubu -c Release

dotnet test Flubu.Tests
