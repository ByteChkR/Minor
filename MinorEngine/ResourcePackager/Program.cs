using System;
using System.IO;
using System.Xml;

namespace ResourcePackager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string csfile = args[0];
            string dir = Path.GetDirectoryName(csfile);
            string folder = args[1];
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

            XmlDocument doc = new XmlDocument();

            File.Move(csfile, csfile + ".backup");

            doc.Load(csfile + ".backup");
            XmlNode n = FindTag(doc);


            for (int i = 0; i < files.Length; i++)
            {
                string cont = Path.GetRelativePath(dir, files[i]);
                n.InnerXml += "\n" + GenerateFileEntry(cont);
            }

            doc.Save(csfile);
        }

        static XmlNode FindTag(XmlDocument doc)
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

        public static string GenerateFileEntry(string filepath)
        {
            return "  <EmbeddedResource Include=\"" + filepath.Replace("\\", "/") + "\" />";
        }
    }
}
