mkdir EngineBuildTools
echo .pdb>>.\temp.txt
echo .config>>.\temp.txt
xcopy .\Engine.BuildTools.Builder.CLI\bin\Release .\EngineBuildTools\ /Y /EXCLUDE:.\temp.txt
xcopy .\Engine.BuildTools.Builder.GUI\bin\Release .\EngineBuildTools\ /Y /EXCLUDE:.\temp.txt
xcopy ..\Engine.Player\Engine.Player\bin\Release .\EngineBuildTools\ /Y /EXCLUDE:.\temp.txt
del .\temp.txt
pause