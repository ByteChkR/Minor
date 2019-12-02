using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.IO
{
    public static class IOManager
    {
        public static bool Exists(string filename)
        {
            bool isFile = File.Exists(filename);
            bool isManifest = ManifestReader.Exists(filename);
            return isFile || isManifest;
        }

        public static bool FolderExists(string foldername)
        {
            bool isFile = Directory.Exists(foldername);
            bool isManifest = ManifestReader.DirectoryExists(foldername);
            return isFile || isManifest;
        }

        public static string[] ReadAllLines(string path)
        {
            return ReadAllText(path).Split('\n');
        }

        public static string ReadAllText(string path)
        {
            TextReader tr = new StreamReader(GetStream(path));
            string ret = tr.ReadToEnd();
            tr.Close();
            return ret;
        }

        public static string[] GetFiles(string foldername, string searchPattern)
        {
            bool folderExists = FolderExists(foldername);

            if (!folderExists)
            {
                Logger.Crash(new InvalidFilePathException(foldername), false);
                return null;
            }

            List<string> files = new List<string>();
            if (Directory.Exists(foldername))
            {
                Logger.Log(foldername + " Found in File System.", DebugChannel.Log, 5);
                files = Directory.GetFiles(foldername, searchPattern).ToList();
            }

            if (ManifestReader.DirectoryExists(foldername))
            {
                Logger.Log(foldername + " Found in Assembly Manifest.", DebugChannel.Log, 5);
                files.AddRange(ManifestReader.GetFiles(foldername, searchPattern.Replace("*", "")));
            }

            return files.ToArray();
        }

        public static Stream GetStream(string filename)
        {
            if (File.Exists(filename))
            {
                Logger.Log(filename + " Found in File System.", DebugChannel.Log, 5);
                return File.OpenRead(filename);
            }

            if (ManifestReader.Exists(filename))
            {
                Logger.Log(filename + " Found in Assembly Manifest.", DebugChannel.Log, 5);
                return ManifestReader.GetStreamByPath(filename);
            }

            Logger.Crash(new InvalidFilePathException(filename), false);
            return null;
        }
    }
}