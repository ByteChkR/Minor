namespace Engine.Common
{
    /// <summary>
    /// Interface for the TextProcessor IO Callbacks.
    /// </summary>
    public interface IIoCallback
    {
        bool FileExists(string file);
        string[] ReadAllLines(string file);
        string[] GetFiles(string path, string searchPattern = "*.*");
    }
}