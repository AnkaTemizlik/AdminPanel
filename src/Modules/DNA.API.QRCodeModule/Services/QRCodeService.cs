using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Logging;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.API.QRCodeModule.Services {
    public interface IQRCodeService {
        string GetAsBase64String(string text);
    }

    [Service(typeof(IQRCodeService), Lifetime.Scoped)]
    public class QRCodeService : IQRCodeService {

        public string GetAsBase64String(string text) {
            using QRCodeGenerator generator = new QRCodeGenerator();
            using QRCodeData data = generator.CreateQrCode(text.Trim(), QRCodeGenerator.ECCLevel.L);
            using QRCode code = new QRCode(data);
            using System.Drawing.Bitmap image = code.GetGraphic(20);
            using MemoryStream stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            stream.Close();
            return Convert.ToBase64String(bytes);
        }
    }
}
