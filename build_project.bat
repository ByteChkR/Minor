echo Building Project:

echo Setting up Submodules:

call build_submodules.bat

echo Setting up Engine:

dotnet restore Engine/

echo Building Engine:

dotnet build Engine/ -c Release

echo Running Tests on Engine:

dotnet test Engine/ -p:Configuration=Debug