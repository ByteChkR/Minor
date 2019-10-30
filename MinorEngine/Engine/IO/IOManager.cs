using System.IO;
using System.Reflection;
using Engine.DataTypes;

namespace Engine.IO
{
    public static class IOManager
    {
        public static bool Exists(string filename)
        {
            return File.Exists(filename) || ManifestReader.Exists(Assembly.GetEntryAssembly(), filename);
        }

        public static Stream GetStream(string filename)
        {
            if (File.Exists(filename))
            {
                return File.OpenRead(filename);
            }

            return ManifestReader.GetStreamByPath(Assembly.GetEntryAssembly(), filename);
        }
    }
}