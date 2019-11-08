using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace Engine.Player.BuildTools
{
    public static class AssemblyEmbedder
    {
        public static void UnEmbedFilesFromProject(string csFile)
        {
            string file = csFile + ".backup";
            Thread.Sleep(250);
            if (!File.Exists(file))
            {
                throw new ArgumentException("Invalid Filepath");
            }

            if (File.Exists(csFile))
            {
                File.Delete(csFile);
            }

            File.Move(file, csFile);
        }
        public static void EmbedFilesIntoProject(string csFile, string[] fileList)
        {


            List<Tuple<string, string>> files = ParseFileList(fileList);
            List<string> f = new List<string>();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine("Trying to open File: " + files[i].Item1);
                if (File.Exists(files[i].Item1))
                {
                    f.Add(files[i].Item1);
                }
                else if (Directory.Exists(files[i].Item1))
                {
                    string pattern = files[i].Item2;
                    string path = files[i].Item1;
                    Console.WriteLine("Searching Folder Path: " + path);
                    Console.WriteLine("Adding Files with Pattern: " + pattern);
                    f.AddRange(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
                }
            }

            XmlDocument doc = new XmlDocument();
            string filename = csFile + ".backup";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            File.Copy(csFile, filename);

            doc.Load(csFile);
            XmlNode n = FindTag(doc);

            Uri pdir = new Uri(Path.GetDirectoryName(csFile) + "/" + Path.GetFileNameWithoutExtension(csFile));
            for (int i = 0; i < f.Count; i++)
            {
                Uri furi = new Uri(f[i]);
                string cont = pdir.MakeRelativeUri(furi).ToString();
                string entry = GenerateFileEntry(cont);

                Console.WriteLine("Adding File to csproj File: " + cont);
                if (!n.InnerXml.Contains(entry))
                {
                    n.InnerXml += "\n" + entry;
                }
            }

            File.Delete(csFile);
            doc.Save(csFile);
        }

        private static List<Tuple<string, string>> ParseFileList(string[] args)
        {
            string[] lines;
            if (args[0].StartsWith("@"))
            {
                Console.WriteLine("Reading File: " + args[1].Replace("@", ""));
                lines = File.ReadAllLines(args[1].Replace("@", ""));
            }
            else
            {
                List<string> l = new List<string>();
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine("Adding File: " + args[i]);
                    l.Add(args[i]);
                }

                lines = l.ToArray();
            }


            List<Tuple<string, string>> ret = new List<Tuple<string, string>>();
            for (int i = 0; i < lines.Length; i++)
            {
                string[] kvp = lines[i].Split('+');
                if (kvp.Length == 1)
                {
                    ret.Add(new Tuple<string, string>(kvp[0], "*"));
                }

                else
                {
                    for (int j = 1; j < kvp.Length; j++)
                    {
                        string pattern = kvp[j];
                        if (pattern.StartsWith("*"))
                        {
                            pattern = pattern.Remove(0, 1);
                        }

                        ret.Add(new Tuple<string, string>(kvp[0], kvp[j]));
                    }
                }
            }

            return ret;
        }

        private static XmlNode FindTag(XmlDocument doc)
        {
            string s1 = doc.Name;
            XmlNode s = doc.FirstChild;

            for (int i = 0; i < s.ChildNodes.Count; i++)
            {
                if (s.ChildNodes[i].Name == "ItemGroup")
                {
                    if (s.ChildNodes[i].HasChildNodes && s.ChildNodes[i].FirstChild.Name == "EmbeddedResource")
                    {
                        s.ChildNodes[i].InnerXml = "";
                        return s.ChildNodes[i];
                    }
                }
            }

            XmlNode n = doc.CreateNode(XmlNodeType.Element, "ItemGroup", "");
            s.AppendChild(n);

            return n;
        }

        private static string GenerateFileEntry(string filepath)
        {
            return "  <EmbeddedResource Include=\"" + filepath.Replace("\\", "/") + "\" />";
        }

    }
}
