[![GuardRails badge](https://badges.guardrails.io/ByteChkR/Minor.svg?token=f4224ee3848c490228cdebeec7fcff181c192abb1787060a44821b3f584ae47e)](https://dashboard.guardrails.io/default/gh/ByteChkR/Minor)[![Codacy Badge](https://api.codacy.com/project/badge/Grade/8a9a300bcf0d4a04af01841077792532)](https://www.codacy.com/manual/ByteChkR/Minor?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ByteChkR/Minor&amp;utm_campaign=Badge_Grade)[![Build Status](https://travis-ci.com/ByteChkR/Minor.svg?branch=master)](https://travis-ci.com/ByteChkR/Minor)[![codecov](https://codecov.io/gh/ByteChkR/Minor/branch/master/graph/badge.svg)](https://codecov.io/gh/ByteChkR/Minor)

# Minor Project 

[<img src="https://github.com/ByteChkR/Minor/raw/develop/.resources/trello_img.png" width=20 height=20>Trello Planning Board](https://trello.com/b/ioMq8ZzG/minor-todo)

## Minor Engine Installation instructions

### Required Files:
These files can be installed manually or can be 

GraphicsCard that supports OpenGL 3 and OpenCL 1.2
Nvidia and AMD include their implementations with their drivers(no setup needed)
Alternatively the Dll can be dropped in the release folder of Minor/MinorEngine/MinorEngine

Git for Windows
This is only required to have the git command in regular windows CMD (for all the batch scripts to work)
https://git-scm.com/download/win

.NET Core SDK >= 2.2
For compiling / running C# code
https://dotnet.microsoft.com/download/dotnet-core/2.2


### Installation:
#### Engine Installer:
Is the install script. When launched it will ask the user to specify a branch at origin
It will clone the git repo and sub repos and call start_engine.bat on the repositoy.

#### Install_$Branchname$:
Is a shortcut that skips the user input by piping the $Branchname$ into the batch file.

#### Start Engine:
Has a (poor) check if the project has been built, and will call build_project.bat
Starts the engine

#### Build Project:
Calls build_submodules.bat
and builds/tests the Engine

### Installation Manual:

Visual Studio:

	1. Build Physics Engine(bepuphysics2/Library.sln in release mode)
	2. Build ADL(Not possible to build all modules since a few of them require .NET Framework):
	- ADL/ADL/ADL/ADL.csproj in release mode
	- ADL/ADL/ADL.Crash/ADL.Crash.csproj in release mode
	- ADL/ADL/ADL.Network.Shared/ADL.Network.Shared.csproj in release mode
	- ADL/ADL/ADL.Network.Client/ADL.Network.Client.csproj in release mode
	- ADL/ADL/ADL.Network.Server/ADL.Network.Server.csproj in release mode
	3. Build ext_pp Preprocessor(ext-pp/ext-pp.sln in release mode)
	4. Build opencl-dot-net wrapper(opencl-dotnet/OpenCl.DotNetCore.sln in release mode)
	5. Build and Start MinorEngine/MinorEngine.sln 

## Once Started:
C opens the console.
	h / help is listing all the commands available

### Commands:
	mov <x> <y> <z> -> Moves the camera by xyz amount
	rot <x> <y> <z> <angle in degrees> -> rotates the camera around xyz axis by angle in degrees
	rain <object count> -> Spawn a number of colliders
	gravity <x> <y> <z> -> Set global gravity
	runfl <filename> -> runs a FL script (all the scripts contained in filter/*)
	dbgfl <filename> -> starts a debugging session
	step -> executes the next instruction and renders the output
	dbgstop/r -> aborts the current debugging session
	help/h -> shows the help text
	exit/quit/q -> disables the console
	clr/clear -> clears the console output
	ltex -> Lists all textures that were loaded from disk(also lists the reference count)

## Feature List
- OpenGL 3 Rendering
- [OpenCL](https://github.com/ByteChkR/opencl-dotnet)
- [BepuPhysics Engine 2](https://github.com/bepu/bepuphysics2)
- OpenAL 3D Audio
- Render Targets
- Custom YAML interpreter language called OpenFL to use OpenCL Filters(ShaderGraph text based)
- [Fast Debugging Framework](https://github.com/ByteChkR/adl)
- [Preprocessing on GL/CL/FL Scripts](https://github.com/ByteChkR/ext-pp)
- Layer Systems
- UI System
- Font Rendering
- all major image formats
- all major mesh formats(and partly embedded textures)
- wave file loading
- Debugging Console
by Tim Akermann