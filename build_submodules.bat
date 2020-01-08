

echo Setting up ADL:

dotnet restore ADL/ADL/

echo Setting up ext-pp:

dotnet restore ext-pp/

echo Setting up Command Runner:

dotnet restore CommandRunner/CommandRunner/CommandRunner.csproj

echo Building ADL Project:

dotnet build ADL/ADL/ADL/ADL.csproj -c Release
dotnet build ADL/ADL/ADL.Crash/ADL.Crash.csproj -c Release
dotnet build ADL/ADL/ADL.Network.Client/ADL.Network.Client.csproj -c Release
dotnet build ADL/ADL/ADL.Network.Shared/ADL.Network.Shared.csproj -c Release
dotnet build ADL/ADL/ADL.UnitTests/ADL.UnitTests.csproj -c Release

echo Building Command Runner

dotnet build CommandRunner/CommandRunner/CommandRunner.csproj -c Release


echo Building ext-pp Project:

dotnet build ext-pp/ext_pp.sln -c Release