using AutoMapper;
using DNA.Domain.Exceptions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PointmentApp.Exceptions;
using PointmentApp.Models;
using PointmentApp.Services;
using System;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PointmentApp.Controllers {
    [Route("api/[Controller]")]
    [Produces("application/json")]
    [Authorize]
    [ApiController]
    public class AppointmentController : ControllerBase {

        private readonly ILogger<AppointmentController> _logger;
        private readonly IProcessService _processService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValuerService _valuerService;
        private readonly ITranslationService _translationService;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _services;
        private readonly IEntityService _entityService;

        public AppointmentController(IConfiguration configuration, ILogger<AppointmentController> logger, IMapper mapper, IServiceProvider services, IEntityService entityService, IProcessService processService, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IValuerService valuerService, ITranslationService translationService) {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _services = services;
            _entityService = entityService;
            _processService = processService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _valuerService = valuerService;
            _translationService = translationService;
        }
        /*
        [HttpPost]
        [ProducesResponseType(typeof(Response<Appointment>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> PostAsync([FromBody] Appointment resource) {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                if (resource == null)
                    throw new Alert(AlertCodes.ValueCanNotBeEmpty, ("Field", "Invoice"));

                _logger.LogInformation(AlertCodes.ControllerIncomingData, ("action", "api/invoice"), ("resource", JsonConvert.SerializeObject(resource)));

                //if (string.IsNullOrWhiteSpace(resource.))
                //    throw new Alert(AlertCodes.ValueCanNotBeEmpty, ("Field", "Invoice.InvoiceCode"));

                Response response = new DNA.Domain.Services.Communication.Response();

                var appointment = _mapper.Map<Appointment, Appointment>(resource);

                await Task.CompletedTask;
                
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Alert ex) {
                _logger.LogError(ex, ex.Message);
                return BadRequest(new Response(ex));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GeneralError, ex);
                return BadRequest(new Response(alert));
            }
        }
        */

        [HttpPost("{id}/file")]
        [ProducesResponseType(typeof(Response<Document>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> UploadDocument(int id) {
            try {
                var name = Guid.NewGuid().ToString();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", $"{id}");
                var document = new Document { AppointmentId = id };
                if (Request.Form.Files != null && Request.Form.Files.Count > 0) {
                    var file = Request.Form.Files[0];
                    var fileName = Path.ChangeExtension(name, Path.GetExtension(file.FileName));
                    var fileFullName = Path.Combine(path, fileName);

                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    using (var fileStream = System.IO.File.Create(fileFullName)) {
                        await file.CopyToAsync(fileStream);
                        document.Size = Math.Round(fileStream.Length / 1024.0, 2);
                    }

                    const int thumbnailSize = 360;
                    using FileStream stream = System.IO.File.Open(fileFullName, FileMode.Open);
                    var image = Image.FromStream(stream);
                    document.Width = image.Width;
                    document.Height = image.Height;

                    var thumbnail = $"{Path.GetFileNameWithoutExtension(fileName)}-thumbnail{Path.GetExtension(fileName)}";

                    //using var fileStream = System.IO.File.OpenRead(@"TestData\dog.jpg");
                    using var outputStream = new MemoryStream();
                    var actialSizes = ResizeImage(thumbnailSize, image, outputStream);
                    using (var newImage = Image.FromStream(outputStream)) {
                        // Save the resized image to disk
                        newImage.Save(Path.Combine(path, thumbnail));
                    }
                    //using var resized = new Bitmap(width, height);
                    //using var graphics = Graphics.FromImage(resized);
                    //graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //graphics.CompositingMode = CompositingMode.SourceCopy;
                    //graphics.DrawImage(image, 0, 0, width, height);

                    //using var output = System.IO.File.Open(Path.Combine(path, thumbnail), FileMode.Create);
                    //using var encoderParameters = new EncoderParameters(1);
                    //encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
                    //ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(z => z.MimeType == file.ContentType);
                    //resized.Save(output, codec, encoderParameters);

                    document.Width = actialSizes.Width;
                    document.Height = actialSizes.Height;
                    document.ThumbnailUrl = $"/files/{id}/{thumbnail}";
                    document.FileType = file.ContentType;
                    document.Name = file.FileName;
                    document.Url = $"/files/{id}/{fileName}";

                    await _processService.InsertAsync(document);

                    return Ok(new Response(document));
                }
                else
                    throw new Exception("There is no file to save.");
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        [HttpDelete("file/{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> DeleteDocument(int id) {
            try {
                var document = await _processService.GetAsync<Document>(id);
                
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", $"{document.AppointmentId}");
                var fileName = Path.Combine(path, Path.GetFileName(document.Url));
                var thumbnailName = Path.Combine(path, Path.GetFileName(document.ThumbnailUrl));
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
                if (System.IO.File.Exists(thumbnailName))
                    System.IO.File.Delete(thumbnailName);

                var count = await _entityService.DeleteAsync("Document", new { Id = id });
                return Ok(new Response<bool>(count > 0));
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;

        Size ResizeImage(int thumbnailSize, Image image, Stream output) {
            int width, height;
            if (image.Width > image.Height) {
                width = thumbnailSize;
                height = Convert.ToInt32(image.Height * thumbnailSize / (double)image.Width);
            }
            else {
                width = Convert.ToInt32(image.Width * thumbnailSize / (double)image.Height);
                height = thumbnailSize;
            }
            var targetSize = new TargetSize(width, height);

            // Calculate the resize factor
            var scaleFactor = targetSize.CalculateScaleFactor(image.Width, image.Height);
            //scaleFactor /= (int)multiplier;
            var actialSize = new Size(image.Width, image.Height);

            var newWidth = (int)Math.Floor(image.Width / scaleFactor);
            var newHeight = (int)Math.Floor(image.Height / scaleFactor);
            using (var newBitmap = new Bitmap(newWidth, newHeight)) {
                using (var imageScaler = Graphics.FromImage(newBitmap)) {
                    imageScaler.CompositingQuality = CompositingQuality.HighQuality;
                    imageScaler.SmoothingMode = SmoothingMode.HighQuality;
                    imageScaler.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    imageScaler.DrawImage(image, imageRectangle);

                    // Fix orientation if needed.
                    if (image.PropertyIdList.Contains(OrientationKey)) {
                        var orientation = (int)image.GetPropertyItem(OrientationKey).Value[0];
                        switch (orientation) {
                            case NotSpecified: // Assume it is good.
                            case NormalOrientation: // No rotation required.
                                break;
                            case MirrorHorizontal:
                                newBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                break;
                            case UpsideDown:
                                newBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                break;
                            case MirrorVertical:
                                newBitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                                break;
                            case MirrorHorizontalAndRotateRight:
                                newBitmap.RotateFlip(RotateFlipType.Rotate90FlipX);
                                actialSize = new Size(image.Height, image.Width);
                                break;
                            case RotateLeft:
                                newBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                actialSize = new Size(image.Height, image.Width);
                                break;
                            case MirorHorizontalAndRotateLeft:
                                newBitmap.RotateFlip(RotateFlipType.Rotate270FlipX);
                                actialSize = new Size(image.Height, image.Width);
                                break;
                            case RotateRight:
                                newBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                actialSize = new Size(image.Height, image.Width);
                                break;
                            default:
                                throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
                        }
                    }
                    newBitmap.Save(output, image.RawFormat);
                }

            }
            return actialSize;
        }

        // Class definition for the class used in the method above
        public class TargetSize {
            /// <summary>
            /// The _width
            /// </summary>
            private readonly int _width;

            /// <summary>
            /// The _height
            /// </summary>
            private readonly int _height;

            /// <summary>
            /// Initializes a new instance of the <see cref="TargetSize"/> class.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public TargetSize(int width, int height) {
                _height = height;
                _width = width;
            }

            /// <summary>
            /// Calculates the scale factor.
            /// </summary>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            /// <returns></returns>
            public decimal CalculateScaleFactor(int width, int height) {
                // Scale proportinately
                var heightScaleFactor = decimal.Divide(height, _height);
                var widthScaleFactor = decimal.Divide(width, _width);

                // Use the smaller of the two as the final scale factor so the image is never undersized.
                return widthScaleFactor > heightScaleFactor ? heightScaleFactor : widthScaleFactor;
            }
        }
    }
}
