using AutoMapper;
using DNA.Domain.Exceptions;
using DNA.Domain.Extentions;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PointmentApp.Models;
using S8.SmsModule.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PointmentApp.Services {
    public interface IAppointmentService {
        Task<Response> DeleteDocumentAsync(int id);
        Task<Response> UploadDocumentAsync(int id, Microsoft.AspNetCore.Http.IFormFileCollection files);
        Task<Response> InsertAppointmentAsync(Appointment appointment);
        Task<Response> UpdateAppointmentAsync(int id, dynamic appointment);
    }

    [Service(typeof(IAppointmentService), Lifetime.Scoped)]
    public class AppointmentService : IAppointmentService {

        private readonly ILogger<AppointmentService> _logger;
        private readonly IProcessService _processService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IValuerService _valuerService;
        private readonly ISmsService _smsService;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _services;
        private readonly IEntityService _entityService;

        public AppointmentService(IConfiguration configuration, ILogger<AppointmentService> logger, IMapper mapper, IServiceProvider services, IEntityService entityService, IProcessService processService, IEmailService emailService, IValuerService valuerService, ISmsService smsService) {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _services = services;
            _entityService = entityService;
            _processService = processService;
            _emailService = emailService;
            _valuerService = valuerService;
            _smsService = smsService;
        }

        public async Task<Response> DeleteDocumentAsync(int id) {
            var document = await _processService.GetAsync<Document>(id);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", $"{document.AppointmentId}");
            var fileName = Path.Combine(path, Path.GetFileName(document.Url));
            var thumbnailName = Path.Combine(path, Path.GetFileName(document.ThumbnailUrl));
            if (File.Exists(fileName))
                File.Delete(fileName);
            if (File.Exists(thumbnailName))
                File.Delete(thumbnailName);

            var count = await _entityService.DeleteAsync("Document", new { Id = id });
            return new Response<bool>(count > 0);
        }

        public async Task<Response> UpdateAppointmentAsync(int id, dynamic appointment) {
            try {
                JObject jObject = JObject.FromObject(appointment);
                if (jObject.ContainsKey("StartDate"))
                    await CheckOneDayAppointmentCount(jObject.ToObject<Appointment>());
                await _entityService.UpdateAsync("Appointment", id, appointment);
                await AddMessage(await _processService.GetAsync<Appointment>(id));
                return new Response();
            }
            catch (Exception ex) {
                var alert = _logger.LogError(ex);
                return new Response(alert);
            }
        }

        public async Task<Response> InsertAppointmentAsync(Appointment appointment) {
            try {
                await CheckOneDayAppointmentCount(appointment);
                appointment.Id = await _processService.InsertAsync(appointment);
                await AddMessage(appointment);
                return new Response();
            }
            catch (Exception ex) {
                var alert = _logger.LogError(ex);
                return new Response(alert);
            }
        }

        async Task<int> CheckOneDayAppointmentCount(Appointment appointment) {
            var result = await _processService.FirstAsync<DNA.Domain.Models.KeyValue>(SqlQueries.CheckOneDayAppointmentCount, appointment);
            if (result.Key > 0) {
                var section = _configuration.GetSection("Config:AppointmentSettings");
                var numberOfAppointmentsAllowedPerDay = section.GetValue<int>("NumberOfAppointmentsAllowedPerDay");
                if (result.Key >= numberOfAppointmentsAllowedPerDay) {
                    throw new Exception("İzin verilen günlük randevu sayısı aşıldı.");
                }
            }
            return result.Key;
        }

        async Task AddMessage(Appointment appointment) {
            // for filling gaps
            var customer = await _processService.GetAsync<Customer>(appointment.CustomerId);
            var service = await _processService.GetAsync<Service>(appointment.ServiceId);
            _valuerService.SetCurrentModel(appointment);
            _valuerService.SetCurrentModel(customer);
            _valuerService.SetCurrentModel(service);

            await AddCreationInfoMessage(appointment);
            await AddCheckStateChangeMessage(appointment);
        }

        async Task AddCreationInfoMessage(Appointment appointment) {
            var sectionAppointmentPlaningInformationMessage = _configuration.GetSection("Config:SmsSettings:AppointmentPlaningInformationMessage");
            var appointmentPlaningInformationMessageEnabled = sectionAppointmentPlaningInformationMessage.GetValue<bool>("Enabled");

            if (appointmentPlaningInformationMessageEnabled) {
                if (appointment.Id > 0 && appointment.State == sectionAppointmentPlaningInformationMessage.GetValue<AppointmentState>("State")) {
                    await AddCreationSmsAsync(appointment);
                }
            }
        }

        async Task AddCheckStateChangeMessage(Appointment appointment) {
            try {
                var sectionStateChangeMessage = _configuration.GetSection("Config:SmsSettings:StateChangeMessage");
                var stateChangeMessageEnabled = sectionStateChangeMessage.GetValue<bool>("Enabled");

                if (stateChangeMessageEnabled) {
                    // add sms
                    if (appointment.Id > 0 && appointment.State == sectionStateChangeMessage.GetValue<AppointmentState>("State")) {
                        await AddRememberSmsAsync(appointment);
                    }

                    // delete unsents if not necessary
                    var deleteUnsentIfStatusNotAvailable = sectionStateChangeMessage.GetValue<bool>("DeleteUnsentIfStatusNotAvailable");
                    if (deleteUnsentIfStatusNotAvailable && appointment.State != sectionStateChangeMessage.GetValue<AppointmentState>("State")) {
                        var appointmentSms = await _processService.QueryAsync<AppointmentSms>(SqlQueries.SelectSmsForPlaning, appointment);
                        foreach (var item in appointmentSms) {
                            await _processService.ExecuteAsync(SqlQueries.DeleteSms, item);
                        }
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(AlertCodes.GeneralError, ex, ("Appointment", appointment.Id));
            }
        }

        // AppointmentPlaningInformationMessage **********
        async Task<int> AddCreationSmsAsync(Appointment appointment) {
            if (appointment.StartDate > DateTime.Now) {
                var sectionCreation = _configuration.GetSection("Config:SmsSettings:AppointmentPlaningInformationMessage");
                var sendInMinutes = sectionCreation.GetValue<int>("SendInMinutes");
                var creationInfoTime = DateTime.Now.AddMinutes(sendInMinutes);
                var to = _valuerService.Get(sectionCreation.GetValue<string>("PhoneNumber"));
                var text = _valuerService.Get(sectionCreation.GetValue<string>("Text"));
                if (creationInfoTime > appointment.StartDate)
                    creationInfoTime = DateTime.Now.AddMinutes(10);

                // check if exists not sent sms for this appointment
                var creationAppointmentSms = await _processService.FirstAsync<AppointmentSms>(SqlQueries.SelectSmsForCreation, appointment);
                if (creationAppointmentSms != null) {
                    await _smsService.UpdateAsync(creationAppointmentSms.SmsId, to, text, creationInfoTime);
                }
                else {
                    // add. 2 = 
                    var smsId = await _smsService.InsertAsync(to, text, creationInfoTime, 2);
                    if (smsId > 0) {
                        await _processService.InsertAsync(new AppointmentSms {
                            AppointmentId = appointment.Id,
                            SmsId = smsId
                        });
                    }
                }
            }
            return 0;

        }

        // StateChangeMessage ************
        async Task<int> AddRememberSmsAsync(Appointment appointment) {

            // başlama zamanı henüz gelmemişse ekleme yapma, sadece ileri tarihli ilerde sms planla
            if (appointment.StartDate > DateTime.Now) {

                var sectionStateChangeMessage = _configuration.GetSection("Config:SmsSettings:StateChangeMessage");

                var sendPreviousDayAtThisHour = sectionStateChangeMessage.GetValue<int>("SendPreviousDayAtThisHour");
                var sendBeforeThisHour = sectionStateChangeMessage.GetValue<int>("SendBeforeThisHour");
                var scheduledSendingTime = sendPreviousDayAtThisHour > 0
                    ? new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, appointment.StartDate.Day).AddDays(-1).AddHours(sendPreviousDayAtThisHour)
                    : sendBeforeThisHour > 0
                        ? appointment.StartDate.AddHours(sendBeforeThisHour)
                        : (DateTime?)null;

                // add Sms

                // planlanan sms zamanı şimdiden daha küçükse SendBeforeThisHour kullan
                if (scheduledSendingTime < DateTime.Now)
                    scheduledSendingTime = appointment.StartDate.AddHours(sendBeforeThisHour);

                // hata planlanan sms zamanı şimdiden daha küçükse 15 dakika sonra göndermeye kur
                if (scheduledSendingTime > appointment.StartDate)
                    scheduledSendingTime = DateTime.Now.AddMinutes(15);

                var to = _valuerService.Get(sectionStateChangeMessage.GetValue<string>("PhoneNumber"));
                var text = _valuerService.Get(sectionStateChangeMessage.GetValue<string>("Text"));

                // check if exists not sent sms for this appointment
                var planingAppointmentSms = await _processService.FirstAsync<AppointmentSms>(SqlQueries.SelectSmsForPlaning, appointment);
                if (planingAppointmentSms != null) {
                    await _smsService.UpdateAsync(planingAppointmentSms.SmsId, to, text, scheduledSendingTime);
                }
                else {
                    // add AppointmentSms
                    var smsId = await _smsService.InsertAsync(to, text, scheduledSendingTime, 1);
                    if (smsId > 0) {
                        await _processService.InsertAsync(new AppointmentSms {
                            AppointmentId = appointment.Id,
                            SmsId = smsId
                        });
                    }
                }
            }
            return 0;
        }

        public async Task<Response> UploadDocumentAsync(int id, Microsoft.AspNetCore.Http.IFormFileCollection files) {

            var name = Guid.NewGuid().ToString();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files", $"{id}");
            var document = new Document { AppointmentId = id };
            if (files != null && files.Count > 0) {
                var file = files[0];
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

                return new Response(document);
            }
            else
                throw new Exception("There is no file to save.");

        }

        #region Image Processes

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

        #endregion
    }
}
