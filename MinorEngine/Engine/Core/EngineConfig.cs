using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Assimp;
using Engine.Debug;
using Engine.Exceptions;

namespace Engine.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigVariable : Attribute
    {
    }

    [Serializable]
    public class EngineConfig
    {
        public static void CreateConfig(Assembly asm, string nameSpace, string filePath)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;


            List<Tuple<string, string>> serializedObjs = CreateConfigObjects(asm, nameSpace, xws);

            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.AppendChild(doc.CreateNode(XmlNodeType.Element, "Settings", ""));
            foreach (Tuple<string, string> serializedObj in serializedObjs)
            {
                XmlNode container = node.AppendChild(doc.CreateNode(XmlNodeType.Element, serializedObj.Item1, ""));


                TextReader tr = new StringReader(serializedObj.Item2);

                XmlDocument cont = new XmlDocument();
                cont.LoadXml(serializedObj.Item2);


                container.AppendChild(doc.ImportNode(cont.FirstChild, true));


                //content.OuterXml = serializedObj.Item2;
                Console.Write("AAAA");
            }

            TextWriter tw = new StringWriter();
            XmlWriter xwr = XmlWriter.Create(tw, xws);

            doc.Save(xwr);

            string config = Regex.Replace(tw.ToString(), NamespaceMatch, "");
            File.WriteAllText(filePath, config);
        }

        public static void CreateConfig(object obj, string filePath)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;


            List<Tuple<string, string>> serializedObjs = CreateConfigObjects(obj, xws);

            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.AppendChild(doc.CreateNode(XmlNodeType.Element, "Settings", ""));
            foreach (Tuple<string, string> serializedObj in serializedObjs)
            {
                XmlNode container = node.AppendChild(doc.CreateNode(XmlNodeType.Element, serializedObj.Item1, ""));


                TextReader tr = new StringReader(serializedObj.Item2);

                XmlDocument cont = new XmlDocument();
                cont.LoadXml(serializedObj.Item2);


                container.AppendChild(doc.ImportNode(cont.FirstChild, true));

                
            }

            TextWriter tw = new StringWriter();
            XmlWriter xwr = XmlWriter.Create(tw, xws);

            doc.Save(xwr);

            string config = Regex.Replace(tw.ToString(), NamespaceMatch, "");
            File.WriteAllText(filePath, config);
        }

        public static void LoadConfig(string configName, ref object obj)
        {
            if (!File.Exists(configName))
            {
                Logger.Crash(new InvalidFilePathException(configName), true);
                return;
            }

            List<Tuple<string, MemberInfo>> serializedObjs =
                GetPropertiesWithAttribute(obj.GetType(), BindingFlags.Instance | BindingFlags.Public).ToList();
            XmlDocument doc = new XmlDocument();
            doc.Load(configName);
            foreach (Tuple<string, MemberInfo> serializedObj in serializedObjs)
            {
                XmlNode node = GetObject(doc, serializedObj.Item1 + "." + serializedObj.Item2.Name);
                if (node != null)
                {
                    XmlSerializer xserializer = new XmlSerializer(GetMemberType(serializedObj.Item2));
                    TextReader tr = new StringReader(node.InnerXml);
                    XmlReader xr = XmlReader.Create(tr);
                    SetValue(serializedObj.Item2, obj, xserializer.Deserialize(xr));
                }
            }
        }

        public static void LoadConfig(string configName, Assembly asm, string nameSpace)
        {
            if (!File.Exists(configName))
            {
                Logger.Crash(new InvalidFilePathException(configName), true);
                return;
            }

            List<Tuple<string, MemberInfo>> serializedObjs =
                GetPropertiesWithAttribute<ConfigVariable>(asm, nameSpace).ToList();
            XmlDocument doc = new XmlDocument();
            doc.Load(configName);
            foreach (Tuple<string, MemberInfo> serializedObj in serializedObjs)
            {
                XmlNode node = GetObject(doc, serializedObj.Item1 + "." + serializedObj.Item2.Name);
                if (node != null)
                {
                    XmlSerializer xserializer = new XmlSerializer(GetMemberType(serializedObj.Item2));
                    TextReader tr = new StringReader(node.InnerXml);
                    XmlReader xr = XmlReader.Create(tr);
                    SetValue(serializedObj.Item2, null, xserializer.Deserialize(xr));
                }
            }
        }

        private static XmlNode GetObject(XmlDocument doc, string fullname)
        {
            return doc.SelectSingleNode("Settings/" + fullname);
        }

        private const string NamespaceMatch = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";

        private static List<Tuple<string, string>> CreateConfigObjects(Assembly asm, string nameSpace,
            XmlWriterSettings settings)
        {
            Tuple<string, MemberInfo>[] info = GetPropertiesWithAttribute<ConfigVariable>(asm, nameSpace);
            List<Tuple<string, string>> serializedVars = new List<Tuple<string, string>>();
            XmlSerializer xs;
            for (int i = 0; i < info.Length; i++)
            {
                xs = new XmlSerializer(GetMemberType(info[i].Item2));
                TextWriter tw = new StringWriter();
                XmlWriter xw = XmlWriter.Create(tw, settings);
                xs.Serialize(xw, GetValue(info[i].Item2, null));
                string serializedObj = tw.ToString();
                serializedVars.Add(new Tuple<string, string>(info[i].Item1 + "." + info[i].Item2.Name, serializedObj));
            }


            return serializedVars;
        }


        private static List<Tuple<string, string>> CreateConfigObjects(object obj, XmlWriterSettings settings)
        {
            Tuple<string, MemberInfo>[] info =
                GetPropertiesWithAttribute(obj.GetType(), BindingFlags.Public | BindingFlags.Instance);
            List<Tuple<string, string>> serializedVars = new List<Tuple<string, string>>();
            XmlSerializer xs;
            for (int i = 0; i < info.Length; i++)
            {
                xs = new XmlSerializer(GetMemberType(info[i].Item2));
                TextWriter tw = new StringWriter();
                XmlWriter xw = XmlWriter.Create(tw, settings);
                xs.Serialize(xw, GetValue(info[i].Item2, obj));
                string serializedObj = tw.ToString();
                serializedVars.Add(new Tuple<string, string>(info[i].Item1 + "." + info[i].Item2.Name, serializedObj));
            }


            return serializedVars;
        }


        private static object GetValue(MemberInfo info, object reference)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)info).GetValue(reference);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)info).GetValue(reference);
            }

            return null;
        }

        private static void SetValue(MemberInfo info, object reference, object value)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                ((FieldInfo)info).SetValue(reference, value);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo)info).SetValue(reference, value);
            }
        }

        private static Type GetMemberType(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)info).FieldType;
            }

            if (info.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)info).PropertyType;
            }

            return null;
        }


        private static Tuple<string, MemberInfo>[] GetPropertiesWithAttribute(Type t, BindingFlags flags)
        {
            List<FieldInfo> fis = t.GetFields(flags).ToList();
            List<Tuple<string, MemberInfo>> ret = new List<Tuple<string, MemberInfo>>();

            foreach (FieldInfo fieldInfo in fis)
            {
                foreach (CustomAttributeData fieldInfoCustomAttribute in fieldInfo.CustomAttributes)
                {
                    if (fieldInfoCustomAttribute.AttributeType == typeof(ConfigVariable))
                    {
                        ret.Add(new Tuple<string, MemberInfo>(t.FullName.Replace("+", "."), fieldInfo));
                        break;
                    }
                }
            }

            List<PropertyInfo> pis = t.GetProperties(flags).ToList();

            foreach (PropertyInfo propertyInfo in pis)
            {
                foreach (CustomAttributeData propertyInfoCustomAttribute in propertyInfo.CustomAttributes)
                {
                    if (propertyInfoCustomAttribute.AttributeType == typeof(ConfigVariable))
                    {
                        ret.Add(new Tuple<string, MemberInfo>(t.FullName.Replace("+", "."), propertyInfo));
                        break;
                    }
                }
            }

            return ret.ToArray();
        }

        private static Tuple<string, MemberInfo>[] GetPropertiesWithAttribute<T>(Assembly asm, string nameSpace)
            where T : Attribute
        {
            List<Tuple<string, MemberInfo>> ret = new List<Tuple<string, MemberInfo>>();
            IEnumerable<Tuple<string, Type>> namespaceTypes = from t in asm.GetTypes()
                                                              where t.Namespace != null && t.Namespace.Contains(nameSpace)
                                                              select new Tuple<string, Type>(t.FullName.Replace("+", "."), t);


            Type attribType = typeof(T);
            Tuple<string, Type>[] types = namespaceTypes.ToArray();
            foreach (Tuple<string, Type> item in types)
            {
                ret.AddRange(GetPropertiesWithAttribute(item.Item2, BindingFlags.Static | BindingFlags.Public));
            }

            return ret.ToArray();
        }
    }
}