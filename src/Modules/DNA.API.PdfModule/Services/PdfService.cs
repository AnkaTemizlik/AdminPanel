using DevExpress.Pdf;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraRichEdit;
using DNA.Domain.Extentions;
using DNA.Domain.Models;
using DNA.Domain.Models.Enums;
using DNA.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNA.API.PdfModule.Services {
    public interface IPdfService {
        void Open(string path);
        string SaveHtmlToPdf(string path, string fileName, string htmlContent, PrintLayoutOrientation orientation = PrintLayoutOrientation.Portrait);
        void SendToPrinter(string fileFullName, PrintSettings settings);
    }

    [Service(typeof(IPdfService), Lifetime.Scoped)]
    public class PdfService : IPdfService {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IPdfService> _logger;

        public PdfService(IConfiguration configuration, ILogger<IPdfService> logger) {
            _configuration = configuration;
            _logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="htmlContent"></param>
        /// <returns>returns file full name</returns>
        public string SaveHtmlToPdf(string path, string fileName, string htmlContent, PrintLayoutOrientation orientation = PrintLayoutOrientation.Portrait) {

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("path", "Pdf dosya yolu boş olamaz.");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileFullName = Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fileName)}.pdf");

            if (File.Exists(fileFullName))
                File.Delete(fileFullName);

            using RichEditDocumentServer documentServer = new RichEditDocumentServer();

            documentServer.LoadDocument(Encoding.UTF8.GetBytes(htmlContent), DocumentFormat.Html);

            PdfExportOptions options = new PdfExportOptions();
            options.DocumentOptions.Author = _configuration["Config:Company:CompanyName"];
            options.Compressed = false;
            options.ImageQuality = PdfJpegImageQuality.Medium;

            var pdfSection = _configuration.GetSection("Config:PdfSettings");
            var pageWidth = pdfSection.GetValue<float>("PageWidth");
            var pageHeight = pdfSection.GetValue<float>("PageHeight");
            var marginSection = pdfSection.GetSection("Margins");
            var marginsBottom = marginSection.GetValue<float>("Bottom");
            var marginsTop = marginSection.GetValue<float>("Top");
            var marginsLeft = marginSection.GetValue<float>("Left");
            var marginsRight = marginSection.GetValue<float>("Right");

            foreach (var docSection in documentServer.Document.Sections) {
                docSection.Page.Landscape = orientation == PrintLayoutOrientation.Landscape;
                docSection.Page.Width = pageWidth > 0 ? pageWidth : docSection.Page.Width;
                docSection.Page.Height = pageHeight > 0 ? pageHeight : docSection.Page.Height;
                docSection.Page.PaperKind = System.Drawing.Printing.PaperKind.A4;
                docSection.Margins.Left = marginsLeft > 0 ? marginsLeft : 10f;
                docSection.Margins.Right = marginsRight > 0 ? marginsRight : 10f;
                docSection.Margins.Top = marginsTop > 0 ? marginsTop : 10f;
                docSection.Margins.Bottom = marginsBottom > 0 ? marginsBottom : 10f;
            }

            using (FileStream pdfFileStream = new FileStream(fileFullName, FileMode.Create)) {
                documentServer.ExportToPdf(pdfFileStream, options);
            }
            _logger.LogInformation($"PDF file created", ("FileName", fileName));
            return fileFullName;
        }

        public void SendToPrinter(string fileFullName, PrintSettings settings) {
            using PdfDocumentProcessor pdfDocumentProcessor = new PdfDocumentProcessor();
            pdfDocumentProcessor.LoadDocument(fileFullName);
            var printerSettings = new PdfPrinterSettings {
                PageOrientation = settings.Orientation == PrintLayoutOrientation.Landscape ? PdfPrintPageOrientation.Landscape : PdfPrintPageOrientation.Portrait,
            };
            printerSettings.Settings.DefaultPageSettings.PaperSize = new PaperSize() { RawKind = settings.PaperKind };
            printerSettings.Settings.PrinterName = settings.Name;
            printerSettings.Settings.Collate = settings.Collate;
            printerSettings.Settings.PrintToFile = false;
            printerSettings.Settings.Copies = settings.CopyCount < 1 ? (short)1 : (short)settings.CopyCount;
            printerSettings.Settings.PrintRange = PrintRange.AllPages;

            _logger.LogInformation($"File printing at {settings.Name}");

            pdfDocumentProcessor.Print(printerSettings);

            _logger.LogInformation($"File sent to printer '{settings.Name}'");
        }

        public void Open(string pdfFullName) {
            
            var process = new Process {
                StartInfo = new ProcessStartInfo(pdfFullName) {
                    UseShellExecute = true,
                    CreateNoWindow = true
                }
            };
            
            _logger.LogInformation($"PDF process stating '{pdfFullName}'");
            
            process.Start();

            _logger.LogInformation($"PDF process started '{pdfFullName}'");
        }
    }
}
