@echo off
echo CSProj File = %1
echo _________________
echo Include Folder + FilePattern = %2
echo _________________
echo Publish Dir = %3
echo _________________
echo Output Dir = %4
echo _________________
echo Project Dir = %5
echo _________________
echo Plain Project Name = %6
echo _________________


cd resources\packager

echo Running Packager for Packs and Asset Folder:
dotnet ResourcePackager.dll %1 %2

cd ..\..

echo _________________

echo Building Project:
dotnet build %1 -c Release

echo Publishing Project:
dotnet publish %1 -c Release

echo _________________

echo Copying Published Content:
xcopy %3 %4 /s /e /y /i /c

cd resources/packager

echo _________________

echo Restoring Project:

dotnet ResourcePackager.dll %1.backup

echo _________________

echo Removing Packed Files from Folder: %5\%6

@RD %5\%6 /S /Q

cd ..\..

echo Build Finished
