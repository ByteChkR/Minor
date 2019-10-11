[![GuardRails badge](https://badges.guardrails.io/ByteChkR/Minor.svg?token=f4224ee3848c490228cdebeec7fcff181c192abb1787060a44821b3f584ae47e)](https://dashboard.guardrails.io/default/gh/ByteChkR/Minor)[![Codacy Badge](https://api.codacy.com/project/badge/Grade/8a9a300bcf0d4a04af01841077792532)](https://www.codacy.com/manual/ByteChkR/Minor?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ByteChkR/Minor&amp;utm_campaign=Badge_Grade)[![Build Status](https://travis-ci.com/ByteChkR/Minor.svg?branch=develop)](https://travis-ci.com/ByteChkR/Minor)[![codecov](https://codecov.io/gh/ByteChkR/Minor/branch/master/graph/badge.svg)](https://codecov.io/gh/ByteChkR/Minor)[<img src="https://github.com/ByteChkR/Minor/raw/develop/.resources/trello_img.png" width=20 height=20>Trello Planning Board](https://trello.com/b/ioMq8ZzG/minor-todo)

# Minor Project 

This is a C# Game Engine Project that is made for the specialization phase for university.

## Features of Version 0.1
* OpenGL Rendering (OpenTK)
* OpenAL 3D Audio (OpenTK)
* An *almost* complete OpenCL Wrapper for .NET Core
* Physics with BepuPhysics v1
* Procedural generation capabilities
* Loading Objects with Assimp.Net
* Different Render Targets
* A UI System
* A integrated debugging console that is extensible
* An assembly like language called OpenFL that is used to chain different CL kernels together to push the creation of procedural content to a more abstract level so that it gets more accessible in game development
* Custom Text Preprocessing for GL Shaders / CL Kernels and FL Scripts
* A fast Mask based Debugging Framework that is able to send logs almost everywhere
* Currently Rather basic IO Loading capabilities
	- Obj/Fbx
	- All major image formats
	- Wave/RIFF audio loading
* A bunch of noise implementations
	- Perlin
	- Smooth
	- Worley
	- ...
* The WaveCollapse Function

## [Project Structure](resources/ProjectStructure.md)

## [Documentation / Reference Pages](https://bytechkr.github.io/Minor/index.html)

## Minor Engine Installation instructions

### Required Files:
These files can be installed manually or can be 

Graphics Card that supports OpenGL 3 and OpenCL 1.2
Nvidia and AMD include their implementations with their drivers(no setup needed)
Alternatively the dll can be dropped in the release folder of Minor/MinorEngine/MinorEngine

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

	1. Build ADL(Not possible to build all modules since a few of them require .NET Framework):
	- ADL/ADL/ADL/ADL.csproj in release mode
	- ADL/ADL/ADL.Crash/ADL.Crash.csproj in release mode
	- ADL/ADL/ADL.Network.Shared/ADL.Network.Shared.csproj in release mode
	- ADL/ADL/ADL.Network.Client/ADL.Network.Client.csproj in release mode
	- ADL/ADL/ADL.Network.Server/ADL.Network.Server.csproj in release mode
	2. Build ext_pp Preprocessor(ext-pp/ext-pp.sln in release mode)
	3. Build and Start MinorEngine/MinorEngine.sln 

### Default Commands for the Console:
	help / h -> Display All commands
	q/quit/exit -> Close the console(Pressing ESC while the text input is empty does the same thing)
	cls/clear -> Clears the console output
	lmem -> Lists the statistics of the different stages of all collected frames
	llmem -> Lists the last complete stage(Update or Render)
	cmd <command> -> Writes the Output of the specified command(including arguments) to the Engine Debug System.
	tg/togglegraph -> shows/hides the delta time graph