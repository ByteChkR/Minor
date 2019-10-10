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

    public class ConfigVariable : Attribute
    {
    }

    [Serializable]
    public class EngineConfig
    {
        public static void CreateConfig(Assembly asm, string nameSpace)
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;


            List<Tuple<string, string>> serializedObjs = CreateConfigObjects(asm, nameSpace, xws);

            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.AppendChild(doc.CreateNode(XmlNodeType.Element, "Settings", ""));
            foreach (var serializedObj in serializedObjs)
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
            File.WriteAllText("test.xml", config);
        }

        public static void LoadConfig(string configName, Assembly asm, string nameSpace)
        {
            if (!File.Exists(configName))
            {
                Logger.Crash(new InvalidFilePathException(configName), true );
                return;
            }

            List<Tuple<string, MemberInfo>> serializedObjs = GetPropertiesWithAttribute<ConfigVariable>(asm, nameSpace).ToList();
            XmlDocument doc = new XmlDocument();
            doc.Load(configName);
            foreach (var serializedObj in serializedObjs)
            {
                XmlNode node = GetObject(doc, serializedObj.Item1 + "." + serializedObj.Item2.Name);
                if (node != null)
                {

                    XmlSerializer xserializer = new XmlSerializer(GetMemberType(serializedObj.Item2));
                    TextReader tr = new StringReader(node.InnerXml);
                    XmlReader xr = XmlReader.Create(tr);
                    SetValue(serializedObj.Item2, xserializer.Deserialize(xr));
                }
            }


        }

        private static XmlNode GetObject(XmlDocument doc, string fullname)
        {
            return doc.SelectSingleNode("Settings/" + fullname);
        }

        private const string NamespaceMatch = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";

        private static List<Tuple<string, string>> CreateConfigObjects(Assembly asm, string nameSpace, XmlWriterSettings settings)
        {

            Tuple<string, MemberInfo>[] info = GetPropertiesWithAttribute<ConfigVariable>(asm, nameSpace);
            List<Tuple<string, string>> serializedVars = new List<Tuple<string, string>>();
            XmlSerializer xs;
            for (int i = 0; i < info.Length; i++)
            {

                xs = new XmlSerializer(GetMemberType(info[i].Item2));
                TextWriter tw = new StringWriter();
                XmlWriter xw = XmlWriter.Create(tw, settings);
                xs.Serialize(xw, GetValue(info[i].Item2));
                string serializedObj = tw.ToString();
                serializedVars.Add(new Tuple<string, string>(info[i].Item1 + "." + info[i].Item2.Name, serializedObj));
            }


            return serializedVars;


        }



        private static object GetValue(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)info).GetValue(null);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)info).GetValue(null);
            }

            return null;
        }

        private static void SetValue(MemberInfo info, object value)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                ((FieldInfo)info).SetValue(null, value);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo)info).SetValue(null, value);
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

        public static void CreateConfig1(Assembly asm, string nameSpace)
        {
            List<string> settings = new List<string>();

            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
            xsn.Add("", "");
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;
            Tuple<string, MemberInfo>[] info = GetPropertiesWithAttribute<ConfigVariable>(asm, nameSpace);

            foreach (var tuple in info)
            {
                TextWriter tw = new StringWriter();

                XmlWriter xw = XmlWriter.Create(tw, xws);
                object obj = null;
                Type t = null;
                if (tuple.Item2.MemberType == MemberTypes.Field)
                {
                    t = ((FieldInfo)tuple.Item2).FieldType;
                    obj = ((FieldInfo)tuple.Item2).GetValue(null);
                }
                else if (tuple.Item2.MemberType == MemberTypes.Property)
                {

                    t = ((PropertyInfo)tuple.Item2).PropertyType;
                    obj = ((PropertyInfo)tuple.Item2).GetValue(null);
                }

                if (t != null)
                {

                    XmlSerializer xs = new XmlSerializer(t);
                    xs.Serialize(xw, obj);
                    settings.Add(tuple.Item1 + "." + tuple.Item2.Name + ":" + tw.ToString());
                }


            }
            TextWriter twr = new StringWriter();
            XmlWriter xwr = XmlWriter.Create(twr, xws);
            XmlSerializer xsr = new XmlSerializer(typeof(List<string>));
            Stream s = File.OpenWrite("test.xml");
            xsr.Serialize(xwr, settings);
            s.Close();
        }

        public static Tuple<string, MemberInfo>[] GetPropertiesWithAttribute<T>(Assembly asm, string nameSpace) where T : Attribute
        {
            List<Tuple<string, MemberInfo>> ret = new List<Tuple<string, MemberInfo>>();
            var namespaceTypes = from t in asm.GetTypes()
                                 where t.Namespace != null && t.Namespace.Contains(nameSpace)
                                 select new Tuple<string, Type>(t.FullName, t);



            Type attribType = typeof(T);
            Tuple<string, Type>[] types = namespaceTypes.ToArray();
            foreach (var item in types)
            {
                //ret.AddRange(item.Item2.GetFields().Where(x => x.CustomAttributes.Where(y => y.AttributeType == attribType).ToList().Count != 0).Select(c => new Tuple<string, MemberInfo>(item.Item1, c)));

                List<FieldInfo> fis = item.Item2.GetFields().ToList();
                List<Tuple<string, MemberInfo>> fisfiltered = new List<Tuple<string, MemberInfo>>();

                foreach (FieldInfo fieldInfo in fis)
                {
                    foreach (CustomAttributeData fieldInfoCustomAttribute in fieldInfo.CustomAttributes)
                    {
                        if (fieldInfoCustomAttribute.AttributeType == typeof(ConfigVariable))
                        {
                            fisfiltered.Add(new Tuple<string, MemberInfo>(item.Item1, fieldInfo));
                            break;
                        }
                    }
                }
                ret.AddRange(fisfiltered);
                //ret.AddRange(item.Item2.GetProperties().Where(x => x.CustomAttributes.Where(y => y.AttributeType == attribType).ToList().Count != 0).Select(c => new Tuple<string, MemberInfo>(item.Item1, c)));
                List<PropertyInfo> pis = item.Item2.GetProperties().ToList();
                List<Tuple<string, MemberInfo>> pisfiltered = new List<Tuple<string, MemberInfo>>();

                foreach (PropertyInfo propertyInfo in pis)
                {
                    foreach (CustomAttributeData propertyInfoCustomAttribute in propertyInfo.CustomAttributes)
                    {
                        if (propertyInfoCustomAttribute.AttributeType == typeof(ConfigVariable))
                        {
                            pisfiltered.Add(new Tuple<string, MemberInfo>(item.Item1, propertyInfo));
                            break;
                        }
                    }
                }
                ret.AddRange(pisfiltered);

            }

            return ret.ToArray();
        }
    }
}