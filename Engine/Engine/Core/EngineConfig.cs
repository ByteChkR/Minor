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
using Engine.IO;

namespace Engine.Core
{
    /// <summary>
    /// A Custom Attribute that is used to save and load the variable dynamically with reflection
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigVariable : Attribute
    {
    }

    /// <summary>
    /// Static Class that Loads and Saves Properties and Fields marked by the ConfigVariable attribute
    /// </summary>
    [Serializable]
    public static class EngineConfig
    {
        /// <summary>
        /// Creates a Config file of the specified assembly & namespace(other namespaces get ignored)
        /// </summary>
        /// <param name="asm">Assembly that contains the namespace</param>
        /// <param name="nameSpace">The namespace</param>
        /// <param name="filePath">The Filepath where the xml file should be saved</param>
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
            }

            TextWriter tw = new StringWriter();
            XmlWriter xwr = XmlWriter.Create(tw, xws);

            doc.Save(xwr);

            string config = Regex.Replace(tw.ToString(), NamespaceMatch, "");
            File.WriteAllText(filePath, config);
        }

        /// <summary>
        /// Creates a config of the specified object
        /// </summary>
        /// <param name="obj">The object to create the config from</param>
        /// <param name="filePath">The filepath where the xml file should be saved</param>
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

        /// <summary>
        /// Loads a config from disk and applies it to the specified object
        /// </summary>
        /// <param name="configName">The XML file containing the config data</param>
        /// <param name="obj">The object that contains the CustomAttribute on one or more fields/properties</param>
        public static void LoadConfig(string configName, ref object obj)
        {
            if (!IOManager.Exists(configName))
            {
                Logger.Crash(new InvalidFilePathException(configName), true);
                return;
            }

            List<Tuple<string, MemberInfo>> serializedObjs =
                GetPropertiesWithAttribute(obj.GetType(), BindingFlags.Instance | BindingFlags.Public).ToList();
            XmlDocument doc = new XmlDocument();
            doc.Load(IOManager.GetStream(configName));
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

        /// <summary>
        /// Loads a config from disk and applies it to the Assembly and Namespace
        /// </summary>
        /// <param name="configName">The path of the XML File containing the object</param>
        /// <param name="asm">The Assembly containing the namespace</param>
        /// <param name="nameSpace">The namespace</param>
        public static void LoadConfig(string configName, Assembly asm, string nameSpace)
        {
            if (!IOManager.Exists(configName))
            {
                Logger.Crash(new InvalidFilePathException(configName), true);
                return;
            }

            List<Tuple<string, MemberInfo>> serializedObjs =
                GetPropertiesWithAttribute<ConfigVariable>(asm, nameSpace).ToList();
            XmlDocument doc = new XmlDocument();
            doc.Load(IOManager.GetStream(configName));
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

        /// <summary>
        /// Wrapper to Get a xml object from an xml node
        /// </summary>
        /// <param name="doc">the document containing the node</param>
        /// <param name="fullname">The full name of the node</param>
        /// <returns></returns>
        private static XmlNode GetObject(XmlDocument doc, string fullname)
        {
            return doc.SelectSingleNode("Settings/" + fullname);
        }

        /// <summary>
        /// Regex that removes the xmlns and xsi namespaces
        /// </summary>
        private const string NamespaceMatch = @"xmlns(:\w+)?=""([^""]+)""|xsi(:\w+)?=""([^""]+)""";

        /// <summary>
        /// Creates a List of tuple containing the xml full path of the object and the serialized object as string
        /// </summary>
        /// <param name="asm">The assembly where the config is generated from</param>
        /// <param name="nameSpace">The namespace</param>
        /// <param name="settings">The XML Writer settings that are used.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Creates a list of tuple containing the xml full path of the object and the serialized object as string
        /// </summary>
        /// <param name="obj">The object that will be searched for ConfigVariable Attributes</param>
        /// <param name="settings">The XML Writer settings used.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Returns the value of a member that is either a Field or a property
        /// </summary>
        /// <param name="info">The info from System.Reflection that describes the field/propery</param>
        /// <param name="reference">The reference object used to obtain the value</param>
        /// <returns>The value of the field/property</returns>
        private static object GetValue(MemberInfo info, object reference)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo) info).GetValue(reference);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo) info).GetValue(reference);
            }

            return null;
        }

        /// <summary>
        /// Sets the value of a member that is either a Field or a property
        /// </summary>
        /// <param name="info">The info from System.Reflection that describes the field/propery</param>
        /// <param name="reference">The reference object used to obtain the value</param>
        /// <param name="value">The value to be set</param>
        private static void SetValue(MemberInfo info, object reference, object value)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                ((FieldInfo) info).SetValue(reference, value);
            }

            if (info.MemberType == MemberTypes.Property)
            {
                ((PropertyInfo) info).SetValue(reference, value);
            }
        }

        /// <summary>
        /// Returns the Type of the member that is either a field or a property
        /// </summary>
        /// <param name="info">The member info describing the field/property</param>
        /// <returns></returns>
        private static Type GetMemberType(MemberInfo info)
        {
            if (info.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo) info).FieldType;
            }

            if (info.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo) info).PropertyType;
            }

            return null;
        }


        /// <summary>
        /// Returns a list of properties with a specfied attribute
        /// </summary>
        /// <param name="t">Type to be searched</param>
        /// <param name="flags">the binding flags</param>
        /// <returns>a list of properties with a specfied attribute</returns>
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

        /// <summary>
        /// Returns a list of properties with a specfied attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute</typeparam>
        /// <param name="asm">The Assembly that is beeing searched</param>
        /// <param name="nameSpace">The namespace</param>
        /// <returns></returns>
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