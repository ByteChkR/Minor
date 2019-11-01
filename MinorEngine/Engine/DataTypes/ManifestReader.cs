using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using AssetPackaging;
using Assimp;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using OpenTK.Graphics.OpenGL;
using Buffer = System.Buffer;

namespace Engine.DataTypes
{
    public static class ManifestReader
    {
        private static Dictionary<string, AssemblyFile> AssemblyFiles = new Dictionary<string, AssemblyFile>();
        private static List<Assembly> LoadedAssemblies = new List<Assembly>();
        private static List<string> UnpackedFiles = new List<string>();
        public static void RegisterAssembly(Assembly asm)
        {
            if (LoadedAssemblies.Contains(asm)) return;

            LoadedAssemblies.Add(asm);
            string[] files = asm.GetManifestResourceNames();
            Logger.Log("Adding Assembly: " + asm.GetName().Name,
                DebugChannel.Engine | DebugChannel.IO | DebugChannel.Log, 10);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i].Remove(0, (asm.GetName().Name + ".").Length);
                if (AssemblyFiles.ContainsKey(file))
                {
                    Logger.Log("Overwriting File: " + file + " with version from assembly: " + asm.GetName().Name,
                        DebugChannel.Engine | DebugChannel.IO | DebugChannel.Log, 10);
                    AssemblyFiles[file] = new AssemblyFile(files[i], asm);
                }
                else
                {
                    Logger.Log("Add File: " + file + " from assembly: " + asm.GetName().Name,
                        DebugChannel.Engine | DebugChannel.IO | DebugChannel.Log, 10);

                    AssemblyFiles.Add(file, new AssemblyFile(files[i], asm));
                }
            }

        }

        private delegate AssemblyFile AssemblyFileFactory(string file, Assembly asm, AssetPointer ptr);
        private static AssemblyFile FileFactory(string file, Assembly asm, AssetPointer ptr)
        {
            if (asm == null) return new IOPackedAssemblyFile(UnSanitizeFilename(file), ptr);
            return new PackedAssemblyFile(file, asm, ptr);
        }

        private static void PrepareAssemblyFiles(string packPrefix, string[] files, Assembly asm, AssemblyFileFactory factory)
        {
            int indexList = HasPackageFiles(files);
            if (indexList == -1) return;

            Logger.Log("Found Packed Files in Assembly: " + packPrefix, DebugChannel.Log, 10);

            string dir = packPrefix + "/packs";
            if (dir.StartsWith("/")) dir = dir.Remove(0, 1);
            string[] packs = IOManager.GetFiles(dir, "*.pack"); //Get only *.pack files
            Stream[] s = new Stream[packs.Length];
            for (int i = 0; i < packs.Length; i++)
            {
                Logger.Log("Creating Stream from " + packs[i], DebugChannel.Log, 10);
                s[i] = IOManager.GetStream(packs[i]);
            }

            Stream indexStream = IOManager.GetStream(files[indexList]);
            Dictionary<string, Tuple<int, MemoryStream>> filesToUnpack = AssetPacker.UnpackAssets(indexStream, s); //Get Files in the Packs that need to be unpacked to file system
            if (filesToUnpack.Count > 0) UnpackAssets(filesToUnpack);


            Stream idxStream = IOManager.GetStream(files[indexList]);
            List<Tuple<string, AssetPointer>> packedFiles = AssetPacker.GetPointers(idxStream, packs);

            foreach (Tuple<string, AssetPointer> assetPointer in packedFiles)
            {
                string assemblyPath = SanitizeFilename(packPrefix + "/" + assetPointer.Item1);
                string virtualPath = SanitizeFilename(assetPointer.Item2.Path);
                Logger.Log("Parsing Packed File " + assetPointer.Item2.Path + " from " + assemblyPath, DebugChannel.Log, 10);
                if (AssemblyFiles.ContainsKey(virtualPath))
                {
                    Logger.Log("Overwriting File..", DebugChannel.Log, 10);
                    AssemblyFiles[virtualPath] = factory(assemblyPath, asm, assetPointer.Item2); //new PackedAssemblyFile(assemblyPath, asm, assetPointer.Item2);
                }
                else
                {
                    AssemblyFiles.Add(virtualPath, factory(assemblyPath, asm, assetPointer.Item2)); //new PackedAssemblyFile(assemblyPath, asm, assetPointer.Item2)
                }
            }
        }

        public static void PrepareManifestFiles(bool searchFileSystem)
        {

            foreach (Assembly loadedAssembly in LoadedAssemblies)
            {
                if (IOManager.FolderExists(loadedAssembly.GetName().Name + "/packs"))
                    PrepareAssemblyFiles(loadedAssembly.GetName().Name, IOManager.GetFiles(loadedAssembly.GetName().Name + "/packs", "*"), loadedAssembly, FileFactory);

            }

            if (searchFileSystem)
            {
                if (IOManager.FolderExists("packs"))
                    PrepareAssemblyFiles("", IOManager.GetFiles("packs", "*"), null, FileFactory);
            }
        }

        private static void UnpackAssets(Dictionary<string, Tuple<int, MemoryStream>> files)
        {
            Logger.Log($"Parparing to unpack {files.Count} Assets.. ", DebugChannel.Log, 10);
            foreach (KeyValuePair<string, Tuple<int, MemoryStream>> memoryStream in files)
            {

                if (!File.Exists(memoryStream.Key))
                {
                    Logger.Log($"Unpacking: " + memoryStream.Key, DebugChannel.Log, 10);
                    byte[] buf = new byte[memoryStream.Value.Item1];
                    memoryStream.Value.Item2.Position = 0;
                    memoryStream.Value.Item2.Read(buf, 0, buf.Length);

                    List<string> folders = new List<string>();
                    string curFolder = Path.GetDirectoryName(memoryStream.Key);
                    folders.Add(curFolder);
                    while (curFolder.Trim() != "\\")
                    {
                        if (string.IsNullOrEmpty(curFolder)) break;
                        folders.Add(curFolder);
                        curFolder = Path.GetDirectoryName(curFolder);
                    }

                    for (int i = 0; i < folders.Count; i++)
                    {
                        if (!Directory.Exists(folders[i]))
                        {
                            Directory.CreateDirectory(".\\" + folders[i]);
                        }
                    }
                    UnpackedFiles.Add(memoryStream.Key);
                    File.WriteAllBytes(memoryStream.Key, buf);
                }


            }
        }

        private static int HasPackageFiles(string[] files)
        {
            int id = -1;
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith("index.xml"))
                {
                    id = i;
                }
            }

            return id;
        }


        public static Stream GetStreamByPath(string filepath)
        {
            string path = SanitizeFilename(filepath);

            if (!AssemblyFiles.ContainsKey(path))
            {
                Logger.Crash(new EngineException("Could not load default Texture"), false);
                return null;
            }

            return AssemblyFiles[path].GetFileStream();

        }

        public static bool DirectoryExists(string path)
        {
            string p = SanitizeFilename(path);
            foreach (KeyValuePair<string, AssemblyFile> assemblyFile in AssemblyFiles)
            {
                if (assemblyFile.Key.StartsWith(p)) return true;
            }

            return false;
        }


        public static string[] GetFiles(string path, string searchPattern)
        {
            string[] files = AssemblyFiles.Keys.ToArray();
            string p = SanitizeFilename(path);
            List<string> ret = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].StartsWith(p) && (files[i].EndsWith(searchPattern) || searchPattern == "*")) ret.Add(UnSanitizeFilename(files[i]));
            }

            return ret.ToArray();
        }


        public static string UnSanitizeFilename(string filepath)
        {
            string ret = filepath.Replace(".", "/");
            int idx = ret.LastIndexOf("/");
            ret = ret.Remove(idx, 1).Insert(idx, ".");


            return ret;
        }

        public static string SanitizeFilename(string filepath)
        {
            if (filepath[0] == '/' || filepath[0] == '\\') filepath = filepath.Remove(0, 1);
            return filepath.Replace("/", ".").Replace("\\", ".");
        }

        public static bool Exists(string filepath)
        {
            string p = SanitizeFilename(filepath);
            return AssemblyFiles.ContainsKey(p);
        }

        public static void ClearUnpackedFiles()
        {
            for (int i = 0; i < UnpackedFiles.Count; i++)
            {
                Logger.Log("Removing File from Filesystem: " + UnpackedFiles[i], DebugChannel.Log, 10);
                File.Delete(UnpackedFiles[i]);
            }
        }
    }
}