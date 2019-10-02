@echo off

echo Checking Build Status:
IF NOT EXIST "MinorEngine\MinorEngine\bin\Release\netcoreapp2.1\MinorEngine.dll" call build_project.bat

echo Running Engine...
cd MinorEngine\MinorEngine\assets
dotnet run --launch-profile "GameEngine" -p "..\MinorEngine.csproj" -c Release