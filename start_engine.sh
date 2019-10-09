
echo Checking Build Status:

if [ ! -d "MinorEngine\Demo\bin\Release\netcoreapp2.1\Engine.dll" ]; then
  /bin/bash build_project.sh
fi

echo Running Engine...
cd MinorEngine\Demo\assets
dotnet run -p "..\Demo.csproj" -c Release