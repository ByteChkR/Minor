using System.IO;

namespace Engine.BuildTools.PackageCreator.Versions
{
    public interface IPackageVersion
    {
        string ManifestPath { get; }
        string PackageVersion { get; }

        bool IsVersion(string path);

        void CreateGamePackage(string packageName, string executable, string outputFile, string workingDir,
            string[] files, string version);

        void CreateEnginePackage(string outputFile, string workingDir, string[] files, string version = null);

        void UnpackPackage(string file, string outPutDir);
        IPackageManifest GetPackageManifest(string path);

        void WriteManifest(Stream s, IPackageManifest manifest);
    }
}