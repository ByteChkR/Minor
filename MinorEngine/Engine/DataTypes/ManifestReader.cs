using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Assimp;
using Engine.Debug;
using Engine.Exceptions;
using Engine.IO;
using OpenTK.Graphics.OpenGL;

namespace Engine.DataTypes
{
    public static class ManifestReader
    {
        public static Stream GetStreamByPath(Assembly asm, string filepath)
        {
            string path = SanizizeFilename(asm, filepath);
            using (Stream resourceStream = asm.GetManifestResourceStream(path))
            {
                if (resourceStream == null)
                {
                    Logger.Crash(new EngineException("Could not load default Texture"), false);
                    return null;
                }

                byte[] buf = new byte[resourceStream.Length];
                resourceStream.Read(buf, 0, (int)resourceStream.Length);

                MemoryStream ms = new MemoryStream(buf);
                
                resourceStream.Close();
                return ms;
            }
        }

        public static bool DirectoryExists(Assembly asm, string path)
        {
            string[] files = asm.GetManifestResourceNames();
            string p = SanizizeFilename(asm, path);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].StartsWith(p)) return true;
            }

            return false;
        }


        public static string[] GetFiles(Assembly asm, string path, string searchPattern)
        {
            List<string> paths = new List<string>();
            string[] files = asm.GetManifestResourceNames();
            string p = SanizizeFilename(asm, path);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].StartsWith(p) && files[i].EndsWith(searchPattern))
                {
                    paths.Add(UnSanizizeFilename(Assembly.GetEntryAssembly(),  files[i]));
                }
            }

            return paths.ToArray();


        }


        public static string UnSanizizeFilename(Assembly asm, string filepath)
        {
            string ret = filepath.Remove(0, asm.GetName().Name.Length).Replace(".", "/");
            int idx = ret.LastIndexOf("/");
            ret = ret.Remove(idx, 1).Insert(idx, ".");



            return ret;
        }

        public static string SanizizeFilename(Assembly asm, string filepath)
        {
            if (filepath[0] == '/' || filepath[0] == '\\') filepath = filepath.Remove(0, 1);
            return asm.GetName().Name + "." + filepath.Replace("/", ".").Replace("\\", ".");
        }

        public static bool Exists(Assembly asm, string filepath)
        {
            string[] files = asm.GetManifestResourceNames();
            string p = SanizizeFilename(asm, filepath);
            return files.Contains(p);
        }
    }
}