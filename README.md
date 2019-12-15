[![GuardRails badge](https://badges.guardrails.io/ByteChkR/Minor.svg?token=f4224ee3848c490228cdebeec7fcff181c192abb1787060a44821b3f584ae47e)](https://dashboard.guardrails.io/default/gh/ByteChkR/Minor)[![Codacy Badge](https://api.codacy.com/project/badge/Grade/8a9a300bcf0d4a04af01841077792532)](https://www.codacy.com/manual/ByteChkR/Minor?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ByteChkR/Minor&amp;utm_campaign=Badge_Grade)[![Build Status](https://travis-ci.com/ByteChkR/Minor.svg?branch=develop)](https://travis-ci.com/ByteChkR/Minor)[![codecov](https://codecov.io/gh/ByteChkR/Minor/branch/develop/graph/badge.svg)](https://codecov.io/gh/ByteChkR/Minor)[<img src="https://github.com/ByteChkR/Minor/raw/develop/resources/trello_img.png" width=20 height=20>Trello Planning Board](https://trello.com/b/ioMq8ZzG/minor-todo)

# Minor Project 

This is a C# Game Engine Project that is made for the specialization phase for university.

## Change Log v0.2:
* Bugfixes
* FL Features
* A Graph Renderer
* Memory Optimizations
* Refractored Classes to improve coding efficiency
* A Config Loader that works everywhere in the codebase
* More CL Filter
* Texture Offset and Tiling
* Memory Optimizations

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

## [Documentation / Reference Pages](https://bytechkr.github.io/Minor/index.html)

## [Getting Started Pages/Repository](https://github.com/ByteChkR/MinorDemoAndTutorial)

## Minor Engine Installation instructions

### Required Files:
Graphics Card that supports OpenGL 3 and OpenCL 1.2
Nvidia and AMD include their implementations with their drivers(no setup needed)
Alternatively the dll can be dropped in the release folder of Minor/MinorEngine/MinorEngine

Git for Windows
This is only required to have the git command in regular windows CMD (for all the batch scripts to work)
https://git-scm.com/download/win

.NET Core SDK >= 2.1
For compiling / running C# code
https://dotnet.microsoft.com/download/dotnet-core/2.1


# Cloning the repository
1. `git clone --recurse-submodules https://github.com/ByteChkR/Minor`
2. Execute `Minor/build_submodules.bat`