@echo off
echo CSProj File = %1
echo _________________
echo Building Project:
dotnet build %1 -c Release

echo _________________

echo Publishing Project:
dotnet publish %1 -c Release

echo Build Finished
