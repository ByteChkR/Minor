cd ..

version_helper\Engine.BuildTools.VersionHelper.exe --pattern Engine\Engine\Engine.csproj X.X.+.0> temp.txt

set /p NEWVERSION=<temp.txt
del temp.txt
call version_helper\_build.bat %NEWVERSION%