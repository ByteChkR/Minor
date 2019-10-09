@echo off

echo Checking Build Status:
IF NOT EXIST "MinorEngine\Demo\bin\Release\netcoreapp2.1\Engine.dll" call build_project.bat

echo Running Engine...
cd MinorEngine\Demo\assets
dotnet run -p "..\Demo.csproj" -c Release