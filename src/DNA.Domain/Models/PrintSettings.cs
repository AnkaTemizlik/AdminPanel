using DNA.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DNA.Domain.Models {
    public class PrintSettings {

        /// <summary>
        /// Yazıcı adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kopya sayısı
        /// </summary>
        public int CopyCount { get; set; }

        /// <summary>
        /// Oryantasyon: Portrait (dikey) / Landscape (yatay)
        /// </summary>
        public PrintLayoutOrientation Orientation { get; set; }

        /// <summary>
        /// Harmanla
        /// </summary>
        public bool Collate { get; set; }

        /// <summary>
        /// A4 için: <c>9</c>
        /// <para>
        /// <see href="https://docs.microsoft.com/tr-tr/dotnet/api/system.drawing.printing.paperkind?view=netcore-3.1">System.Drawing.Printing.PaperKind</see>
        /// </para>
        /// </summary>
        public int PaperKind { get; set; } = 9;
    }
}
