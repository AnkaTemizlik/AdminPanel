using DNA.Domain.Extentions;
using DNA.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace DNA.API.Services {

    [Service(typeof(IXmlService), Lifetime.Scoped)]
    public class XmlService : IXmlService {
        public string Serialize(object data) {
            XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
            StringBuilder stringBuilder = new StringBuilder();
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            MemoryStream memoryStream = new MemoryStream();
            Encoding encoding = (Encoding)new UTF8Encoding(false);
            TextWriter output = (TextWriter)new StreamWriter((Stream)memoryStream, encoding);
            XmlWriter xmlWriter = XmlWriter.Create(output, new XmlWriterSettings() {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Auto,
                Indent = true
            });
            xmlSerializer.Serialize(xmlWriter, data, namespaces);
            xmlWriter.Close();
            output.Close();
            return encoding.GetString(memoryStream.ToArray());
        }

        public T Deserialize<T>(string xml) {
            try {
                return string.IsNullOrWhiteSpace(xml) ? default(T) : (T)this.Deserialize(xml, typeof(T));
            }
            catch (Exception ex) {
                throw new Exception("XML Object Convert Error", ex);
            }
        }

        public object Deserialize(string xml, Type type) {
            object obj;
            using (TextReader textReader = (TextReader)new StringReader(xml))
                obj = new XmlSerializer(type).Deserialize(textReader);
            return obj;
        }

        public string GetInvoiceXML<T>(T obj) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            namespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            namespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            namespaces.Add("ccts", "urn:un:unece:uncefact:documentation:2");
            namespaces.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
            namespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            namespaces.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2");
            namespaces.Add("ubltr", "urn:oasis:names:specification:ubl:schema:xsd:TurkishCustomizationExtensionComponents");
            namespaces.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
            namespaces.Add("udt", "urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2");
            namespaces.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            MemoryStream memoryStream = new MemoryStream();
            Encoding encoding = (Encoding)new UTF8Encoding(false);
            TextWriter textWriter = (TextWriter)new StreamWriter((Stream)memoryStream, encoding);
            xmlSerializer.Serialize(textWriter, (object)obj, namespaces);
            textWriter.Close();
            return encoding.GetString(memoryStream.ToArray()).Replace("<xml xmlns=\"\" />", "");
        }

        public string GetEnvelopeXML<T>(T obj) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("ef", "http://www.efatura.gov.tr/package-namespace");
            namespaces.Add("sh", "http://www.unece.org/cefact/namespaces/StandardBusinessDocumentHeader");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            MemoryStream memoryStream = new MemoryStream();
            Encoding encoding = (Encoding)new UTF8Encoding(false);
            TextWriter textWriter = (TextWriter)new StreamWriter((Stream)memoryStream, encoding);
            xmlSerializer.Serialize(textWriter, (object)obj, namespaces);
            textWriter.Close();
            return encoding.GetString(memoryStream.ToArray());
        }

        public string GetHTML(string xmlFile, byte[] xsltFile) {
            try {
                string end = new StreamReader((Stream)new MemoryStream(xsltFile)).ReadToEnd();
                XslCompiledTransform compiledTransform = new XslCompiledTransform();
                StringReader stringReader = new StringReader(end);
                using (XmlReader stylesheet = XmlReader.Create((TextReader)stringReader, new XmlReaderSettings() {
                    DtdProcessing = DtdProcessing.Prohibit
                }))
                    compiledTransform.Load(stylesheet);
                StringWriter stringWriter = new StringWriter();
                using (XmlReader input = XmlReader.Create((TextReader)new StringReader(xmlFile)))
                    compiledTransform.Transform(input, (XsltArgumentList)null, (TextWriter)stringWriter);
                return stringWriter.ToString();
            }
            catch (XsltException ex) {
                throw new XsltException("Could not create view.", (Exception)ex);
            }
            catch (Exception ex) {
                throw new XsltException("Could not create view.", ex);
            }
        }
    }
}