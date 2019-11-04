using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Assimp;
using Engine.AssetPackaging;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using Buffer = System.Buffer;

namespace Engine.DataTypes
{
    public static class ManifestReader
    {
        private static Dictionary<string, AssemblyFile> AssemblyFiles = new Dictionary<string, AssemblyFile>();
        private static List<Assembly> LoadedAssemblies = new List<Assembly>();
        private static List<string> UnpackedFiles = new List<string>();

        public static void LoadAssemblyListFromFile(string filepath)
        {
            if (IOManager.Exists(filepath))
            {
                LoadAssemblyList(IOManager.GetStream(filepath));
            }
        }

        public static void LoadAssemblyList(Stream data)
        {
            TextReader tr = new StreamReader(data);

            string list = tr.ReadToEnd();
            tr.Close();
            string[] files = list.Split("\n", StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < files.Length; i++)
            {
                Assembly asm = Assembly.Load(files[i].Replace("\r", "")); //Why windows. Why?
                if (asm != null)
                {
                    Logger.Log("Loading Assembly " + asm.GetName().Name, DebugChannel.Log, 10);
                    RegisterAssembly(asm);
                }
            }
        }

        public static void RegisterAssembly(Assembly asm)
        {
            if (LoadedAssemblies.Contains(asm))
            {
                return;
            }

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
                        DebugChannel.Engine | DebugChannel.IO | DebugChannel.Log, 8);
                    AssemblyFiles[file] = new AssemblyFile(false,files[i], asm);
                }
                else
                {

                    AssemblyFiles.Add(file, new AssemblyFile(false, files[i], asm));
                }
            }

            PrepareManifestFiles(asm);
        }

        private delegate AssemblyFile AssemblyFileFactory(string file, bool compression, Assembly asm, AssetPointer ptr);

        private static AssemblyFile FileFactory(string file,bool compression, Assembly asm, AssetPointer ptr)
        {
            if (asm == null)
            {
                return new IOPackedAssemblyFile(compression,UnSanitizeFilename(file), ptr);
            }

            return new PackedAssemblyFile(compression,file, asm, ptr);
        }

        private static void PrepareAssemblyFiles(string packPrefix, string[] files, Assembly asm,
            AssemblyFileFactory factory)
        {
            int indexList = HasPackageFiles(files);
            if (indexList == -1)
            {
                return;
            }

            Logger.Log("Found Packed Files in Assembly: " + packPrefix, DebugChannel.Log, 10);

            string dir = packPrefix + "/packs";
            if (dir.StartsWith("/"))
            {
                dir = dir.Remove(0, 1);
            }

            string[] packs = IOManager.GetFiles(dir, "*.pack"); //Get only *.pack files
            Stream[] s = new Stream[packs.Length];
            for (int i = 0; i < packs.Length; i++)
            {
                s[i] = IOManager.GetStream(packs[i]);
            }

            Stream indexStream = IOManager.GetStream(files[indexList]);
            Dictionary<string, Tuple<int, MemoryStream>>
                filesToUnpack =
                    AssetPacker.UnpackAssets(indexStream,
                        s); //Get Files in the Packs that need to be unpacked to file system
            if (filesToUnpack.Count > 0)
            {
                UnpackAssets(filesToUnpack);
            }


            Stream idxStream = IOManager.GetStream(files[indexList]);
            List<Tuple<string, AssetPointer>> packedFiles = AssetPacker.GetPointers(idxStream, packs, out bool compression);
            Logger.Log("Parsing " + packedFiles.Count + " File from " + files[indexList] + " in " + packs.Length + " Packages.", DebugChannel.Log, 10);

            foreach (Tuple<string, AssetPointer> assetPointer in packedFiles)
            {
                string assemblyPath = SanitizeFilename(packPrefix + "/" + assetPointer.Item1);
                string virtualPath = SanitizeFilename(assetPointer.Item2.Path);
                if (AssemblyFiles.ContainsKey(virtualPath))
                {
                    Logger.Log("Overwriting File: " + assemblyPath + " => " + virtualPath, DebugChannel.Log, 10);
                    AssemblyFiles[virtualPath] =
                        factory(assemblyPath, compression, asm,
                            assetPointer.Item2); //new PackedAssemblyFile(assemblyPath, asm, assetPointer.Item2);
                }
                else
                {
                    AssemblyFiles.Add(virtualPath,
                        factory(assemblyPath, compression, asm,
                            assetPointer.Item2)); //new PackedAssemblyFile(assemblyPath, asm, assetPointer.Item2)
                }
            }
        }


        private static void PrepareManifestFiles(Assembly loadedAssembly)
        {
            if (IOManager.FolderExists(loadedAssembly.GetName().Name + "/packs"))
            {
                PrepareAssemblyFiles(loadedAssembly.GetName().Name,
                    IOManager.GetFiles(loadedAssembly.GetName().Name + "/packs", "*"), loadedAssembly, FileFactory);
            }
        }

        public static void PrepareManifestFiles(bool searchFileSystem)
        {
            if (searchFileSystem)
            {
                if (IOManager.FolderExists("packs"))
                {
                    PrepareAssemblyFiles("", IOManager.GetFiles("packs", "*"), null, FileFactory);
                }
            }
            else
            {
                foreach (Assembly loadedAssembly in LoadedAssemblies)
                {
                    PrepareManifestFiles(loadedAssembly);
                }
            }
        }

        private static void UnpackAssets(Dictionary<string, Tuple<int, MemoryStream>> files)
        {
            Logger.Log($"Parparing to unpack {files.Count} Assets.. ", DebugChannel.Log, 10);
            foreach (KeyValuePair<string, Tuple<int, MemoryStream>> memoryStream in files)
            {
                bool hasUnpackedVersion = UnpackedFiles.Contains(memoryStream.Key);
                if (hasUnpackedVersion) File.Delete(memoryStream.Key);
                bool shouldWrite = hasUnpackedVersion || !File.Exists(memoryStream.Key);
                if (shouldWrite)
                {
                    byte[] buf = new byte[memoryStream.Value.Item1];
                    memoryStream.Value.Item2.Position = 0;
                    memoryStream.Value.Item2.Read(buf, 0, buf.Length);

                    List<string> folders = new List<string>();
                    string curFolder = Path.GetDirectoryName(memoryStream.Key);
                    folders.Add(curFolder);
                    while (curFolder.Trim() != "\\")
                    {
                        if (string.IsNullOrEmpty(curFolder))
                        {
                            break;
                        }

                        folders.Add(curFolder);
                        curFolder = Path.GetDirectoryName(curFolder);
                    }

                    for (int i = 0; i < folders.Count; i++)
                    {
                        if (!Directory.Exists(folders[i]))
                        {
                            Logger.Log($"Creating folder {folders[i]} on disk.", DebugChannel.Log, 8);
                            Directory.CreateDirectory(".\\" + folders[i]);
                        }
                    }

                    if (!hasUnpackedVersion)
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
                Logger.Crash(new EngineException("Could not load file: " + filepath), false);
                return null;
            }

            return AssemblyFiles[path].GetFileStream();
        }

        public static bool DirectoryExists(string path)
        {
            string p = SanitizeFilename(path);
            foreach (KeyValuePair<string, AssemblyFile> assemblyFile in AssemblyFiles)
            {
                if (assemblyFile.Key.StartsWith(p))
                {
                    return true;
                }
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
                if (files[i].StartsWith(p) && (files[i].EndsWith(searchPattern) || searchPattern == "*"))
                {
                    ret.Add(UnSanitizeFilename(files[i]));
                }
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
            if (filepath[0] == '/' || filepath[0] == '\\')
            {
                filepath = filepath.Remove(0, 1);
            }

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
                Logger.Log("Removing File from Filesystem: " + UnpackedFiles[i], DebugChannel.Log, 8);
                File.Delete(UnpackedFiles[i]);
            }
        }
    }
}