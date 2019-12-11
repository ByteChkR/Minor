# Deploying
To deploy a game to a user there are 2 main options to choose from
* Manual Deployment
* Making use of Build Tools
Each way comes with advantages and disadvantages
## Manual Deployment
### Advantages
* Fast Build Time
* Only dotnet SDK needed
* No Version Dependencies
### Disadvantages
* Needs Command Line to Start
* Assets need to be copied manually
* No Data Protection
* Different Builds will always include the whole engine
### Steps
1. Build your Game with either visual studio in release mode or with command
```
	D:\MyGame\>dotnet build MyGame.csproj -c Release
```
2. Publish your Game with command
```
	D:\MyGame\>dotnet publish MyGame.csproj -c Release
```
3. Navigate to bin/Release/netcoreapp2.1/publish/ and Copy your asset folder into the directory.
4. The Game can now be started by
```
	D:\MyGame\bin\Release\netcoreapp2.1\publish> dotnet .\MyGame.dll
```
## Deploying with BuildTools
### Advantages
* Easy one Click Build after setting up
* Versioning System
* Automated Asset Copying
* Asset Packaging
* Builds will only include Game Code
* No Manual Build Steps needed.
* Asset Embedding
### Disadvantages
* Requires .NET Framework
* Needs Build Tools installed on Developer and Target PC
### Steps (Setup)
1. Download the EngineToolBoxInstaller (No Engine Version needs to be downloaded)
2. Open Engine.BuildTools.Builder.GUI.exe
3. Press "New" and save the BuildSettings file it in the Solution Folder of your Game.
4. Fill in the Data needed
	* Project: Select your Game.csproj file(should be in the same folder as the BuildSettings.xml
	* Assets: Select your Asset Folder containing all the resources of the Game
	* Select the Output Folder
	* File Extensions: Add file extensions (one per line) which will be copied from the asset folder into the Build Folder. (Use * for all files)
	* File Extensions that will be unpacked: These extensions will be unpacked when the Game is starting. (Use * for all files)
	* Check the "Create .game Package" Box to generate a .game File that is executable with the Build Tools
	* Check the "Create .engine Package" Box to generate an .engine File that is using the Engine Version that your Game is using.(Optional since most the packages are available automatically)
		- Specify the Engine Project that is used to create the .engine file.
5. Press Save and overwrite the BuildSettings.xml OR Press Run.
### Steps Creating a Build
1. Open Engine.BuildTools.Builder.GUI.exe
2. Load the BuildSettings.xml for the Game
3. Press Run

After Completion the Build Folder containing the Build Result will open.
The .engine File is containing the engine code and the .net runtime.
The .game File is containing the Game Code and information on what engine version is requested.
When Engine.Player.exe is set up as the standard program for *.game files its enough to just double click it to Start the Game.
