using System;
using System.Xml;

namespace VersionHelper
{
    /// <summary>
    /// Small Internal Program that increases a .csproj files Assembly and File Version
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(args[0]);
            XmlNode[] nodes = FindVersionTags(doc);
            Version v = Version.Parse(nodes[0].InnerText);
            Version newv = new Version(v.Major, v.Minor, v.Build, v.Revision + 1);
            nodes[0].InnerText = newv.ToString();
            nodes[1].InnerText = newv.ToString();
            System.IO.File.Delete(args[0]);
            doc.Save(args[0]);
            Console.Write(newv.ToString());
        }

        private static XmlNode[] FindVersionTags(XmlDocument doc)
        {
            string s1 = doc.Name;
            XmlNode s = doc.ChildNodes[1];
            XmlNode[] ret = new XmlNode[2];
            for (int i = 0; i < s.ChildNodes.Count; i++)
            {
                if (s.ChildNodes[i].Name == "PropertyGroup")
                {
                    if (s.ChildNodes[i].HasChildNodes && s.ChildNodes[i].FirstChild.Name == "TargetFramework")
                    {
                        for (int j = 0; j < s.ChildNodes[i].ChildNodes.Count; j++)
                        {
                            XmlNode projTag = s.ChildNodes[i].ChildNodes[j];
                            if (projTag.Name == "AssemblyVersion")
                            {
                                ret[0] = projTag;
                            }
                            else if (projTag.Name == "FileVersion")
                            {
                                ret[1] = projTag;
                            }
                        }
                    }
                }
            }

            return ret;
        }
    }
}