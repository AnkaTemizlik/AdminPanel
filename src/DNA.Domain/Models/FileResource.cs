using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNA.Domain.Models {
    public class FileResource {
        public string FileUrl { get; set; }
        public string Data { get; set; }
        public string DataType { get; set; }
        public string FileSize { get { return Data == null ? "0 byte" : $"{Data.Length / 1024.0:N2} KB"; } }
        public string FileName { get { return Path.GetFileName(FileUrl); } }
    }

    public class LocalFileResource {

        [JsonIgnore]
        public string FullName { get; set; }
        public byte[] Data { get; set; }
        public string Type { get; set; }
        public string Size { get { return Data == null ? "0 byte" : $"{Data.Length / 1024.0:N2} KB"; } }
        public string ZippedFullName { get; set; }
        public string Name { get { return Path.GetFileName(FullName); } }

        public string PathToUrl(IHttpContextAccessor httpContextAccessor) {
            var request = httpContextAccessor.HttpContext.Request;
            var host = $"http{(request.IsHttps ? "s" : "")}://{request.Host.Value}/";
            var fileUrl = FullName.Replace(AppDomain.CurrentDomain.BaseDirectory, host).Replace("\\", "/");
            return fileUrl;
        }
    }
}
