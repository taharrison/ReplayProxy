using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ReplayMocks
{
    public class XmlSerialiser
    {
        public static object Deserialise(string xml, Type type)
        {
            var xmlSerializer = new XmlSerializer(type);
            var stream = new StringReader(xml);
            
            return xmlSerializer.Deserialize(stream);
        }
        public static string Serialise(object model, Type type, bool xmlDeclaration, bool includeNamespaces)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var xmlSerializer = new XmlSerializer(type);
            var sb = new StringBuilder();
            using (
                XmlWriter stream = XmlWriter.Create(sb,
                    new XmlWriterSettings() { OmitXmlDeclaration = !xmlDeclaration, Indent = true }))
            {
                if (includeNamespaces)
                {
                    xmlSerializer.Serialize(stream, model);
                }
                else
                {
                    xmlSerializer.Serialize(stream, model, ns);
                }
            }
            return sb.ToString();
        }

        public static string SerialiseWithoutNamespacesAndHeaderWithLinebreaks(object model, Type type, string xmlRoot = null)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var xmlSerializer = xmlRoot != null ? new XmlSerializer(type, new XmlRootAttribute(xmlRoot)) : new XmlSerializer(type);
            var sb = new StringBuilder();
            using (XmlWriter stream = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                xmlSerializer.Serialize(stream, model, ns);
            }
            return sb.ToString();
        }

        public static string SerialiseWithoutNamespacesAndHeaderWithoutLinebreaks(object model, Type type, string xmlRoot = null)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var xmlSerializer = xmlRoot != null ? new XmlSerializer(type, new XmlRootAttribute(xmlRoot)) : new XmlSerializer(type);
            var sb = new StringBuilder();
            using (XmlWriter stream = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = false }))
            {
                xmlSerializer.Serialize(stream, model, ns);
            }
            return sb.ToString();
        }

        public static string Serialise<T>(T model, string xmlRoot)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRoot));
            var sb = new StringBuilder();
            var stream = new StringWriter(sb);
            xmlSerializer.Serialize(stream, model);
            return sb.ToString();
        }

        internal static string Serialise(object model)
        {
            var xmlSerializer = new XmlSerializer(model.GetType(), model.GetType().Name);
            var sb = new StringBuilder();
            var stream = new StringWriter(sb);
            xmlSerializer.Serialize(stream, model);
            return sb.ToString();
        }
    }
}
