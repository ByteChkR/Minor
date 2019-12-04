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
            if (args.Length == 3 && args[0] == "--pattern")
            {
                _Main(args);
                return;
            }
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


        private static void _Main(string[] args)
        {
            string csProject = args[1];
            string changeStr = args[2];

            XmlDocument doc = new XmlDocument();
            doc.Load(csProject);
            XmlNode[] nodes = FindVersionTags(doc);
            Version v = Version.Parse(nodes[0].InnerText);
            v = ChangeVersion(v, changeStr);
            nodes[0].InnerText = v.ToString();
            nodes[1].InnerText = v.ToString();
            System.IO.File.Delete(csProject);
            doc.Save(csProject);

            Console.Write(v.ToString());
        }

        private static Version ChangeVersion(Version version, string changeStr)
        {
            string[] subVersions = changeStr.Split('.');
            int[] versions = new[] { version.Major, version.Minor, version.Build, version.Revision };
            for (int i = 0; i < 4; i++)
            {
                string current = subVersions[i];
                if (current == "+")
                {
                    versions[i]++;
                }
                else if (current == "-" && versions[i] != 0)
                {
                    versions[i]--;
                }
                else if (current == "X") continue;
                else
                {
                    versions[i] = int.Parse(current);
                }
            }
            return new Version(versions[0], versions[1], versions[2], versions[3]);
        }

        private static Version FindVersion(XmlDocument doc)
        {
            return Version.Parse(FindVersionTags(doc)[0].InnerText);
        }

        private static XmlNode[] FindVersionTags(XmlDocument doc)
        {
            string s1 = doc.Name;

            XmlNode s = null;

            for (int i = 0; i < doc.ChildNodes.Count; i++)
            {
                if (doc.ChildNodes[i].Name == "Project") s = doc.ChildNodes[i];
            }
            
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