@echo off

echo Setting up ADL:

dotnet restore ADL/ADL/

echo Setting up ext-pp:

dotnet restore ext-pp/

echo Setting up opencl-dotnet:

dotnet restore opencl-dotnet/

echo Building ADL Project:

dotnet build ADL/ADL/ADL/ADL.csproj -c Debug
dotnet build ADL/ADL/ADL.Crash/ADL.Crash.csproj -c Debug
dotnet build ADL/ADL/ADL.Network.Client/ADL.Network.Client.csproj -c Debug
dotnet build ADL/ADL/ADL.Network.Shared/ADL.Network.Shared.csproj -c Debug
dotnet build ADL/ADL/ADL.UnitTests/ADL.UnitTests.csproj -c Debug

echo Building ext-pp Project:

dotnet build ext-pp/ext_pp.sln -c Debug
