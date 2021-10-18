using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNA.Domain.Models {
    public class FileResource {
        public string FileUrl { get; set; }
        public string Data { get; set; }
        public string FileSize { get { return Data == null ? "0 byte" : $"{Data.Length / 1024.0:N2} KB"; } }
        public string FileName { get { return Path.GetFileName(FileUrl); } }
    }
}
