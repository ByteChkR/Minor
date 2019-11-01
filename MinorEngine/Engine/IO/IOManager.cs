using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        public static string[] GetFiles(string foldername, string searchPattern)
        {
            if (Directory.Exists(foldername))
            {
                Logger.Log("File Found in File System.", DebugChannel.Log, 10);
                return Directory.GetFiles(foldername, searchPattern);
            }

            if (ManifestReader.DirectoryExists(foldername))
            {
                Logger.Log("File Found in Assembly Manifest.", DebugChannel.Log, 10);
                return ManifestReader.GetFiles(foldername, searchPattern.Replace("*", ""));
            }

            Logger.Crash(new InvalidFilePathException(foldername), false);
            return null;

        }

        public static Stream GetStream(string filename)
        {

            if (File.Exists(filename))
            {
                Logger.Log("File Found in File System.", DebugChannel.Log, 10);
                return File.OpenRead(filename);
            }

            if (ManifestReader.Exists(filename))
            {
                Logger.Log("File Found in Assembly Manifest.", DebugChannel.Log, 10);
                return ManifestReader.GetStreamByPath(filename);
            }

            Logger.Crash(new InvalidFilePathException(filename), false);
            return null;
        }
    }
}