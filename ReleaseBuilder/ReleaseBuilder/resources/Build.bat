@echo off
echo CSProj File = %1
echo PackFolder + FilePattern = %2
echo AssetFolder + FilePattern = %3
echo Publish Dir = %4
echo Output Dir = %5
echo Project Dir = %6


cd resources\packager

echo Running Packager:
dotnet ResourcePackager.dll %1 %2 %3

cd ..\..

echo Building Project:
dotnet build %1 -c Release
dotnet publish %1 -c Release

@RD %6\packs /S /Q

echo Copying %4 to %5
xcopy %4 %5 /s /e /y /i

cd resources/packager
echo Restoring Initial csproj file:

dotnet ResourcePackager.dll %1.backup

cd ..\..

echo Finished
