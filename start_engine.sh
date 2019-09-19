@echo off

echo Checking Build Status:
if [ ! -f MinorEngine/MinorEngine/bin/Release/netcoreapp2.1/MinorEngine.dll ]; then
    bash build_project.sh
fi

echo Running Engine...
cd MinorEngine\MinorEngine\assets
dotnet run --launch-profile "GameEngine" -p "../MinorEngine.csproj"
pause