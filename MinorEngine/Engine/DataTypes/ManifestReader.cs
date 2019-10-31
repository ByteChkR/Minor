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
            public Assembly Assembly;

            public string File;


            public AssemblyFile(string file, Assembly assembly)
            {
                File = file;
                Assembly = assembly;
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
                string file = files[i].Replace(asm.GetName().Name + ".", "");
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

            Unpack(GetFiles("pack", "*"));
        }

        private static void Unpack(string[] files)
        {
            bool hasPackedFiles = false;
            int id = -1;
            for (int i = 0; i < files.Length; i++)
            {
                Logger.Log("Searching Index List: " + files[i], DebugChannel.Log, 10);
                if (files[i].EndsWith("index.xml"))
                {
                    id = i;
                    hasPackedFiles = true;
                }
            }
            Logger.Log("Has Packed Files: " + hasPackedFiles, DebugChannel.Log, 10);
            if (hasPackedFiles)
            {
                string[] f = GetFiles("packs", ".pack");
                Stream[] s = new Stream[f.Length];
                for (int i = 0; i < f.Length; i++)
                {
                    Logger.Log($"Creating Stream: {f[i]}", DebugChannel.Log, 10);
                    s[i] = GetStreamByPath(f[i]);
                }

                Stream indexStream = GetStreamByPath(files[id]);
                Dictionary<string, Tuple<int, MemoryStream>> ret = AssetPacker.UnpackAssets(indexStream, s);
                Logger.Log($"Unpacking {ret.Count} Assets.. ", DebugChannel.Log, 10);
                foreach (KeyValuePair<string, Tuple<int, MemoryStream>> memoryStream in ret)
                {

                    Logger.Log("File Exsists:" + memoryStream.Key + " : " + File.Exists(memoryStream.Key), DebugChannel.Log, 10);
                    if (!File.Exists(memoryStream.Key))
                    {
                        byte[] buf = new byte[memoryStream.Value.Item1];
                        Logger.Log("BufLength: " + buf.Length, DebugChannel.Log, 10);
                        memoryStream.Value.Item2.Position = 0;
                        memoryStream.Value.Item2.Read(buf, 0, buf.Length);

                        List<string> folders = new List<string>();
                        string curFolder = Path.GetDirectoryName(memoryStream.Key);
                        folders.Add(curFolder);
                        while (curFolder.Trim() != "\\")
                        {
                            if (string.IsNullOrEmpty(curFolder)) break;
                            Logger.Log(curFolder, DebugChannel.Log, 10);
                            folders.Add(curFolder);
                            curFolder = Path.GetDirectoryName(curFolder);
                        }

                        for (int i = 0; i < folders.Count; i++)
                        {
                            if (!Directory.Exists(folders[i]))
                            {
                                Logger.Log("Creating Folder: " + folders[i], DebugChannel.Log, 10);
                                Directory.CreateDirectory(".\\" + folders[i]);
                            }
                        }
                        UnpackedFiles.Add(memoryStream.Key);
                        File.WriteAllBytes(memoryStream.Key, buf);
                    }


                }
            }
        }


        public static Stream GetStreamByPath(string filepath)
        {
            string path = SanitizeFilename(filepath);

            if (!AssemblyFiles.ContainsKey(path))
            {
                Logger.Crash(new EngineException("Could not load default Texture"), false);
                return null;
            }

            Assembly asm = AssemblyFiles[path].Assembly;
            using (Stream resourceStream = asm.GetManifestResourceStream(AssemblyFiles[path].File))
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
                File.Delete(UnpackedFiles[i]);
            }
        }
    }
}