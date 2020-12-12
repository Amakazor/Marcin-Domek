using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Marcin_Domek_Server.Src.Extension
{
    internal static class SerializeXMLExtension
    {
        public static string ToXmlString<T>(this T value, bool removeDefaultXmlNamespaces = true, bool omitXmlDeclaration = true, Encoding encoding = null) where T : class
        {
            XmlSerializerNamespaces namespaces = removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = omitXmlDeclaration,
                CheckCharacters = false
            };

            using (StringWriterWithEncoding stream = new StringWriterWithEncoding(encoding))
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, namespaces);
                return stream.ToString();
            }
        }

        public static void FromXmlString<T>(this T value, string XMLString) where T : class
        {
            var serializer = new XmlSerializer(value.GetType());
            T result;

            using (StringReader reader = new StringReader(XMLString))
            {
                result = (T)serializer.Deserialize(reader);
            }

            value = result;
        }
    }
}
