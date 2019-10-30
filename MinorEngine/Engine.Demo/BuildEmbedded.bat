@echo off

cd packager

echo Running Packager:
dotnet ResourcePackager.dll ..\Engine.Demo.csproj @filelist.txt

cd ..

echo Building Demo Project:
dotnet build

cd packager
echo Restoring Initial csproj file:

dotnet ResourcePackager.dll ..\Engine.Demo.csproj.backup

cd ..

echo Finished

pause