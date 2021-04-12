using System;

namespace DNA.Domain.Services {
    public interface IXmlService {
        string Serialize(object objectInstance);
        T Deserialize<T>(string objectData);
        object Deserialize(string objectData, Type type);
        string GetInvoiceXML<T>(T obj);
        string GetEnvelopeXML<T>(T obj);
        string GetHTML(string xmlFile, byte[] xsltFile);
    }
}
