mkdir EngineBuildOutput
echo %1
C:\Engine.Player\Engine.BuildTools.Builder.CLI.exe --create-engine-package ..\Minor\Engine\Engine\Engine.csproj .\EngineBuildOutput\%1.engine
C:\Engine.Player\Engine.Player.exe -l %~dp0..\EngineBuildOutput\%1.engine -nH
if exist ..\UploadFiles\upload.bat (
	copy %~dp0..\EngineBuildOutput\%1.engine ..\UploadFiles\%1.engine
	cd ..\UploadFiles\
	call upload.bat %1
)