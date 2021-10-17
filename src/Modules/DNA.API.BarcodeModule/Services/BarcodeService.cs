using DNA.Domain.Extentions;
using DNA.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BarcodeLib;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace DNA.API.BarcodeModule.Services {
    public interface IBarcodeService {
        string GetAsBase64String(string barcodeText);
    }

    [Service(typeof(IBarcodeService), Lifetime.Scoped)]
    public class BarcodeService : IBarcodeService {

        int Width { get; set; } = 160;
        int Height { get; set; } = 60;
        TYPE BarcodType => TYPE.CODE128;
        LabelPositions LabelPosition { get; set; } = LabelPositions.BOTTOMCENTER;
        RotateFlipType RotateFlipType { get; set; } = RotateFlipType.RotateNoneFlipNone;
        AlignmentPositions AlignmentPositions { get; set; } = AlignmentPositions.CENTER;
        Color BackColor { get; set; } = Color.White;
        Color ForeColor { get; set; } = Color.Black;

        public string GetAsBase64String(string barcodeText) {
            string resultString;
            try {
                using var barcode = new Barcode {
                    EncodedType = BarcodType,
                    IncludeLabel = true,
                    Width = Width,
                    Height = Height,
                    LabelPosition = LabelPosition,
                    RotateFlipType = RotateFlipType
                };

                var image = barcode.Encode(BarcodType, barcodeText, ForeColor, BackColor, Width, Height);

                using var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);
                resultString = Convert.ToBase64String(stream.ToArray());
            }
            catch (Exception ex) {
                throw ex;
            }
            return resultString;
        }
    }
}
