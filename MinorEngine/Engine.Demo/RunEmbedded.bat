@echo off

cd packager

echo Running Packager:
dotnet ResourcePackager.dll ..\Engine.Demo.csproj @filelist.txt

echo Copying Files to Debug Directory
mkdir ..\bin\Debug\netcoreapp2.1\assets
mkdir ..\bin\Debug\netcoreapp2.1\assets\kernel
xcopy ..\assets\kernel ..\bin\Debug\netcoreapp2.1\assets\kernel /s /e

mkdir ..\bin\Debug\netcoreapp2.1\assets\filter
xcopy ..\assets\filter ..\bin\Debug\netcoreapp2.1\assets\filter /s /e

pause
cd ..\bin\Debug\netcoreapp2.1\

echo Running Demo Project:
dotnet run -p ..\..\..\Engine.Demo.csproj

cd ..\..\..\packager

echo Restoring Initial csproj file:

dotnet ResourcePackager.dll ..\Engine.Demo.csproj.backup

cd ..

echo Deleting Files to Debug Directory
@RD /S /Q "bin\Debug\netcoreapp2.1\assets"

echo Finished

pause