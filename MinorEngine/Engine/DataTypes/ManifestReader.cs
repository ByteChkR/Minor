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
        private class AssemblyFile
        {
            public readonly Assembly Assembly;

            public readonly string ManifestFilepath;


            public AssemblyFile(string manifestFilepath, Assembly assembly)
            {
                ManifestFilepath = manifestFilepath;
                Assembly = assembly;
            }

            public virtual Stream GetFileStream()
            {
                using (Stream resourceStream = Assembly.GetManifestResourceStream(ManifestFilepath))
                {
                    if (resourceStream == null)
                    {
                        Logger.Crash(new EngineException("Could not load Manifest File: " + ManifestFilepath), false);
                        return null;
                    }

                    byte[] buf = new byte[resourceStream.Length];
                    resourceStream.Read(buf, 0, (int)resourceStream.Length);

                    MemoryStream ms = new MemoryStream(buf);
                    Logger.Log("Loaded Stream Length: " + ms.Length, DebugChannel.Log, 10);
                    resourceStream.Close();
                    return ms;
                }

            }
        }

        private class PackedAssemblyFile : AssemblyFile
        {
            private AssetPointer ptr;
            public PackedAssemblyFile(string manifestFilepath, Assembly assembly, AssetPointer ptr) : base(manifestFilepath, assembly)
            {
                this.ptr = ptr;
            }


            public override Stream GetFileStream()
            {
                Stream s = base.GetFileStream();
                s.Position = ptr.Offset;
                byte[] buf = new byte[ptr.Length];
                s.Read(buf, 0, ptr.Length);
                s.Close();
                return new MemoryStream(buf);
            }
        }

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

            //Unpack(GetFiles(asm.GetName().Name + "/packs", "*"), asm.GetName().Name);



        }

        public static void PrepareManifestFiles()
        {
            foreach (Assembly loadedAssembly in LoadedAssemblies)
            {
                string packPrefix = loadedAssembly.GetName().Name;
                string[] files = GetFiles(packPrefix + "/packs", "*"); //Get All files
                int indexList = HasPackageFiles(files);
                if (indexList == -1) continue;

                Logger.Log("Found Packed Files in Assembly: " + loadedAssembly.GetName().Name, DebugChannel.Log, 10);

                string[] packs = GetFiles(packPrefix + "/packs", ".pack"); //Get only *.pack files
                Stream[] s = new Stream[packs.Length];
                for (int i = 0; i < packs.Length; i++)
                {
                    Logger.Log("Creating Stream from " + packs[i], DebugChannel.Log, 10);
                    s[i] = GetStreamByPath(packs[i]);
                }

                Stream indexStream = GetStreamByPath(files[indexList]);
                Dictionary<string, Tuple<int, MemoryStream>> filesToUnpack = AssetPacker.UnpackAssets(indexStream, s); //Get Files in the Packs that need to be unpacked to file system
                if (filesToUnpack.Count > 0) UnpackAssets(filesToUnpack);


                Stream idxStream = GetStreamByPath(files[indexList]);
                List<Tuple<string, AssetPointer>> packedFiles = AssetPacker.GetPointers(idxStream, packs);

                foreach (Tuple<string, AssetPointer> assetPointer in packedFiles)
                {
                    string assemblyPath = SanitizeFilename(packPrefix + "/" + assetPointer.Item1);
                    string virtualPath = SanitizeFilename(assetPointer.Item2.Path);
                    Logger.Log("Parsing Packed File " + assetPointer.Item2.Path + " from " + assemblyPath, DebugChannel.Log, 10);
                    if (AssemblyFiles.ContainsKey(virtualPath))
                    {
                        Logger.Log("Overwriting File..", DebugChannel.Log, 10);
                        AssemblyFiles[virtualPath] = new PackedAssemblyFile(assemblyPath, loadedAssembly, assetPointer.Item2);
                    }
                    else
                    {
                        AssemblyFiles.Add(virtualPath, new PackedAssemblyFile(assemblyPath, loadedAssembly, assetPointer.Item2));
                    }
                }

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


            Assembly asm = AssemblyFiles[path].Assembly;
            using (Stream resourceStream = asm.GetManifestResourceStream(AssemblyFiles[path].ManifestFilepath))
            {
                if (resourceStream == null)
                {
                    Logger.Crash(new EngineException("Could not load default Texture"), false);
                    return null;
                }

                byte[] buf = new byte[resourceStream.Length];
                resourceStream.Read(buf, 0, (int)resourceStream.Length);

                MemoryStream ms = new MemoryStream(buf);
                Logger.Log("Loaded Stream Length: " + ms.Length, DebugChannel.Log, 10);
                resourceStream.Close();
                return ms;
            }
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
            Logger.Log("Searching for File: " + p, DebugChannel.Log, 10);
            foreach (KeyValuePair<string, AssemblyFile> assemblyFile in AssemblyFiles)
            {
                //Logger.Log("File: " + assemblyFile.Key, DebugChannel.Log, 10);
            }

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