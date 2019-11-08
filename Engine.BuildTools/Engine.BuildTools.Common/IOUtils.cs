using System.Collections.Generic;
using System.IO;

namespace Engine.BuildTools.Common
{
    public static class IOUtils
    {
        public static void CreateDirectoryPath(string dirPath)
        {
            List<string> folders = new List<string>();
            string curFolder = dirPath;
            folders.Add(curFolder);
            while (!string.IsNullOrEmpty(curFolder) && curFolder.Trim() != "\\")
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
                    Directory.CreateDirectory(".\\" + folders[i]);
                }
            }

        }
    }
}