cd packager
dotnet ResourcePackager.dll ..\Engine.Demo.csproj @filelist.txt
cd ..
dotnet run
cd packager
dotnet ResourcePackager.dll ..\Engine.Demo.csproj.backup
cd ..
pause