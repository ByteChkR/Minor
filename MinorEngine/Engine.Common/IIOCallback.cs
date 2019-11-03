namespace Engine.Common
{
    public interface IIOCallback
    {
        bool FileExists(string file);
        string[] ReadAllLines(string file);
        string[] GetFiles(string path, string searchPattern = "*.*");
    }
}