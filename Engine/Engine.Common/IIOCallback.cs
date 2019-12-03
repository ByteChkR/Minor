namespace Engine.Common
{
    public interface IIoCallback
    {
        bool FileExists(string file);
        string[] ReadAllLines(string file);
        string[] GetFiles(string path, string searchPattern = "*.*");
    }
}