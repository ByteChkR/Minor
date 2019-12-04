cd ..

version_helper\Engine.BuildTools.VersionHelper.exe --pattern Engine\Engine\Engine.csproj X.X.+.0> temp.txt

set /p NEWVERSION=<temp.txt
del temp.txt
mkdir EngineBuildOutput
C:\Engine.Player\Engine.BuildTools.Builder.CLI.exe --create-engine-package ..\Minor\Engine\Engine\Engine.csproj .\EngineBuildOutput\%NEWVERSION%.engine
C:\Engine.Player\Engine.Player.exe -l %~dp0..\EngineBuildOutput\%NEWVERSION%.engine
