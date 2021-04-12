using System.Collections.Generic;

namespace DNA.Domain.Services {
    public interface IZipService {
        Dictionary<string, byte[]> ReadZipFile(byte[] zipFile);
        byte[] ZipFile(string xml, string fileName);
        byte[] ZipFile(byte[] buffer, string fileName, string extension);
    }
}
