cd flubu.core
dotnet restore
dotnet pack
cd ..

cd dotnet-flubu
dotnet restore
dotnet pack

cd ..
mkdir flubu.console\nuget
copy flubu.core\bin\Debug\*.nupkg flubu.console\nuget
copy dotnet-flubu\bin\Debug\*.nupkg flubu.console\nuget

cd flubu.console
dotnet restore -f nuget
cd ..