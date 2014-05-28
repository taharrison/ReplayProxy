using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ReplayMocks
{
    public class Parameter : IXmlSerializable
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public string ParameterActualType { get; set; }
        public string ParameterExpectedType { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Name = reader["Name"];
            ParameterActualType = reader["ParameterActualType"];
            ParameterExpectedType = reader["ParameterExpectedType"];
            reader.Read();
            reader.ReadStartElement("Value");
            Value = XmlSerialiser.Deserialise(reader.ReadOuterXml(), Type.GetType(ParameterActualType));

            reader.ReadEndElement();
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            var type = Value != null ? Value.GetType() : Type.GetType(ParameterActualType);

            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("ParameterActualType", type.AssemblyQualifiedName);
            writer.WriteAttributeString("ParameterExpectedType", ParameterExpectedType);
            writer.WriteStartElement("Value");
            writer.WriteRaw(XmlSerialiser.Serialise(Value, type, false, true));
            writer.WriteEndElement();
        }
    }
}