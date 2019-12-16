namespace Engine.BuildTools.PackageCreator.Versions
{
    /// <summary>
    /// Interface of the Package Manifest
    /// </summary>
    public interface IPackageManifest
    {
        string PackageVersion { get; set; }
        string Title { get; set; }
        string StartCommand { get; set; }
        string Version { get; set; }
    }
}