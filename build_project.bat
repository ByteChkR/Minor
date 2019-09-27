@echo off

echo Building Project:

echo Setting up Submodules:

call build_submodules.bat

echo Setting up MinorEngine:

dotnet restore MinorEngine/

echo Building MinorEngine:

dotnet build MinorEngine/ -c Release

echo Running Tests on MinorEngine:

dotnet test MinorEngine/ -p:Configuration=Release
