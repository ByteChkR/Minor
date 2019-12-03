using System;
using System.Collections.Generic;
using System.IO;

namespace Engine.BuildTools.Common
{
    public static class IoUtils
    {
        public static void CreateDirectoryPath(string dirPath)
        {
            List<string> folders = new List<string>();
            string curFolder = dirPath;
            while (!string.IsNullOrEmpty(curFolder) && curFolder.Trim() != "\\" && curFolder.Trim() != "/")
            {
                if (string.IsNullOrEmpty(curFolder))
                {
                    break;
                }

                Console.WriteLine("Adding Folder to Create List:" + curFolder);
                folders.Add(curFolder);
                curFolder = Path.GetDirectoryName(curFolder);
            }

            for (int i = 0; i < folders.Count; i++)
            {
                if (!Directory.Exists(folders[i]))
                {
                    Directory.CreateDirectory(folders[i]);
                }
            }
        }
    }
}