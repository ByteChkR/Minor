using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.DataTypes;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.IO
{
    /// <summary>
    /// Wrapper for Specific System.IO Calls
    /// It will resolve the filename either with files from the disk or with files embedded in an assembly
    /// </summary>
    public static class IOManager
    {
        /// <summary>
        /// Returns true if the file exists on either the disk or in the assembly
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool Exists(string filename)
        {
            bool isFile = File.Exists(filename);
            bool isManifest = ManifestReader.Exists(filename);
            return isFile || isManifest;
        }

        /// <summary>
        /// Returns true if the folder exists on either the disk or in the assembly
        /// </summary>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public static bool FolderExists(string foldername)
        {
            bool isFile = Directory.Exists(foldername);
            bool isManifest = ManifestReader.DirectoryExists(foldername);
            return isFile || isManifest;
        }

        /// <summary>
        /// Reads all lines from the file provided
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] ReadAllLines(string path)
        {
            return ReadAllText(path).Split('\n');
        }

        /// <summary>
        /// Reads all Text from a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadAllText(string path)
        {
            TextReader tr = new StreamReader(GetStream(path));
            string ret = tr.ReadToEnd();
            tr.Close();
            return ret;
        }

        /// <summary>
        /// Returns files in a specfied directory
        /// </summary>
        /// <param name="foldername"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the byte stream of the file specified
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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