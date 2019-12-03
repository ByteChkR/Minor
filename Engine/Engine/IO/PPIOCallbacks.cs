using System.IO;
using Engine.Common;

namespace Engine.IO
{
    public class PPIOCallbacks : IIoCallback
    {
        public bool FileExists(string file)
        {
            return IOManager.Exists(file);
        }

        public string[] ReadAllLines(string file)
        {
            TextReader tr = new StreamReader(IOManager.GetStream(file));
            string[] ret = tr.ReadToEnd().Replace("\r", "").Split('\n');
            tr.Close();
            return ret;
        }

        public string[] GetFiles(string path, string searchPattern = "*")
        {
            return IOManager.GetFiles(path, searchPattern);
        }
    }
}