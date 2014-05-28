using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ReplayMocks
{
    public class LoggedCall : IXmlSerializable
    {
        public string MethodName { get; set; }

        public object ReturnValue { get; set; }
        public string ReturnType { get; set; }

        public Parameter[] Parameters { get; set; }

        public string ThrownExceptionType { get; set; }
        public string MethodDeclaringTypeName { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            MethodName = reader["MethodName"];
            MethodDeclaringTypeName = reader["MethodDeclaringTypeName"];
            ReturnType = reader["ReturnType"];
            ThrownExceptionType = reader["ThrownExceptionType"];

            do
            {
                reader.Read();
                if (reader.Name == "ReturnValue")
                {
                    var returnType = Type.GetType(ReturnType);
                    var inner = reader.ReadInnerXml();
                    if (returnType != typeof(void))
                        ReturnValue = XmlSerialiser.Deserialise(inner, returnType);
                }

                if (reader.Name == "Parameters")
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("Parameters");
                        var pl = new List<Parameter>();
                        while (reader.NodeType == XmlNodeType.Element)
                        {
                            var p = (Parameter) XmlSerialiser.Deserialise(reader.ReadOuterXml(), typeof (Parameter));
                            pl.Add(p);
                        }

                        Parameters = pl.ToArray();
                    }
                    else
                    {
                        Parameters = new Parameter[]{};
                    }
                    reader.Read();
                }

            } while (reader.NodeType != XmlNodeType.EndElement);
            reader.ReadEndElement();
        }

       // Debug.WriteLine(reader.NodeType);
        

        public void WriteXml(XmlWriter writer)
        {

            writer.WriteAttributeString("MethodName", MethodName);
            writer.WriteAttributeString("MethodDeclaringTypeName", MethodDeclaringTypeName);
            writer.WriteAttributeString("ReturnType", ReturnType);
            if (!string.IsNullOrWhiteSpace(ThrownExceptionType))
            {
                writer.WriteAttributeString("ThrownExceptionType", ThrownExceptionType);
            }
            writer.WriteStartElement("ReturnValue");
            Type returnType = Type.GetType(ReturnType);
            if (returnType != typeof(void))
            {
                writer.WriteRaw(XmlSerialiser.Serialise(ReturnValue, returnType, false, true));
            }
            writer.WriteEndElement();
            

            writer.WriteStartElement("Parameters");
            if (Parameters != null)
            {
                foreach (var parameter in Parameters)
                {
                    writer.WriteRaw(XmlSerialiser.Serialise(parameter, parameter.GetType(), false, true));
                }
            }
            writer.WriteEndElement();
        }
    }
}