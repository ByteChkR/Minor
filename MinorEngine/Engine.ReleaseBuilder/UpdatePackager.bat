@echo off

cd ..\Engine.ResourcePackager
dotnet build -c Release
cd ..\Engine.ReleaseBuilder
for /R ..\Engine.ResourcePackager\bin\Release\netcoreapp2.1 %%f in (*.dll) do copy %%f resources\packager
for /R ..\Engine.ResourcePackager\bin\Release\netcoreapp2.1 %%f in (*.json) do copy %%f resources\packager
for /R resources\packager %%f in (*.dev.json) do del %%f
