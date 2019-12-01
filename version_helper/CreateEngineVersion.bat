cd ..

dotnet version_helper\VersionHelper.dll Engine\Engine\Engine.csproj > temp.txt
set /p NEWVERSION=<temp.txt
del temp.txt
mkdir EngineBuildOutput
C:\Engine.Player\Engine.BuildTools.Builder.CLI.exe --create-engine-package ..\Minor\Engine\Engine\Engine.csproj .\EngineBuildOutput\%NEWVERSION%.engine
