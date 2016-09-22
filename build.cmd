rmdir %userprofile%\.nuget\packages\dotnet-flubu /S /Q
rmdir %userprofile%\.nuget\packages\flubu.core /S /Q
rmdir %userprofile%\.nuget\packages\.tools\dotnet-flubu /S /Q

dotnet pack Flubu.Core -c Release
dotnet pack dotnet-flubu -c Release

dotnet test Flubu.Tests

rem mkdir flubu.console\nuget
rem copy flubu.core\bin\Debug\*.nupkg test\flubu.console\nuget /Y
rem copy dotnet-flubu\bin\Debug\*.nupkg test\flubu.console\nuget /Y

rem cd test\flubu.console
rem dotnet restore -f nuget
rem cd ..
rem cd ..

rem cd flubu.tests
rem dotnet test
rem cd..