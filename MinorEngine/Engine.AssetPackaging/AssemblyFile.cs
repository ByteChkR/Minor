using System.IO;
using System.Reflection;

namespace Engine.AssetPackaging
{
    public class AssemblyFile
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
                    return null;
                }

                byte[] buf = new byte[resourceStream.Length];
                resourceStream.Read(buf, 0, (int) resourceStream.Length);

                MemoryStream ms = new MemoryStream(buf);
                resourceStream.Close();
                return ms;
            }
        }
    }
}