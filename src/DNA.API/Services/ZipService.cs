using DNA.Domain.Extentions;
using DNA.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNA.API.Services {

    [Service(typeof(IZipService), Lifetime.Scoped)]
    public class ZipService : IZipService {
        public Dictionary<string, byte[]> ReadZipFile(byte[] zipFile) {
            Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
            using (MemoryStream memoryStream1 = new MemoryStream(zipFile)) {
                using (ZipArchive zipArchive = new ZipArchive((Stream)memoryStream1)) {
                    foreach (ZipArchiveEntry entry in zipArchive.Entries) {
                        MemoryStream memoryStream2 = new MemoryStream();
                        entry.Open().CopyTo((Stream)memoryStream2);
                        dictionary.Add(entry.Name, memoryStream2.ToArray());
                    }
                }
            }
            return dictionary.Count != 0 ? dictionary : throw new Exception("Zip dosya içerisinde XML veya Text dosya bulunamadı");
        }

        public byte[] ZipFile(string xml, string fileName) {
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            MemoryStream memoryStream = new MemoryStream();
            using (ZipArchive zipArchive = new ZipArchive((Stream)memoryStream, ZipArchiveMode.Create, true)) {
                Stream stream = zipArchive.CreateEntry(fileName).Open();
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            memoryStream.Position = 0L;
            return memoryStream.ToArray();
        }

        public byte[] ZipFile(byte[] buffer, string fileName, string extension) {
            MemoryStream memoryStream = new MemoryStream();
            using (ZipArchive zipArchive = new ZipArchive((Stream)memoryStream, ZipArchiveMode.Create, true)) {
                Stream stream = zipArchive.CreateEntry(fileName + "." + extension).Open();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
            }
            memoryStream.Position = 0L;
            return memoryStream.ToArray();
        }
    }
}
