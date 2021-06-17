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
using PointmentApp.Resources;
using PointmentApp.Services;
using S8.SmsModule.Services;
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
        private readonly IAppointmentService _appointmentService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AppointmentController(IAppointmentService appointmentService, IConfiguration configuration, ILogger<AppointmentController> logger, IMapper mapper) {
            _appointmentService = appointmentService;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
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
                var response = await _appointmentService.UploadDocumentAsync(id, Request.Form.Files);
                if (!response.Success)
                    return BadRequest(response);
                return Ok(response);
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
                var response = await _appointmentService.DeleteDocumentAsync(id);
                if (!response.Success)
                    return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> AddAppointment(AppointmentResource resource) {
            try {

                var appointment = _mapper.Map<Appointment>(resource);
                appointment.UpdatedBy =
                    appointment.CreatedBy = Convert.ToInt32(User.Identity.Name);
                var response = await _appointmentService.InsertAppointmentAsync(appointment);

                if (!response.Success)
                    return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> UpdateAppointment(int id, dynamic resource) {
            try {
                resource.UpdatedBy = Convert.ToInt32(User.Identity.Name);
                resource.UpdateTime = DateTime.Now;
                var x = resource.StartDate;
                var y = resource.EndDate;
                var response = await _appointmentService.UpdateAppointmentAsync(id, resource);
                if (!response.Success)
                    return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }

        [HttpPut("plan/{id}")]
        [Authorize(Roles = "Admin, Writer")]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> UpdateAppointment(int id) {
            try {
                var updatedBy = Convert.ToInt32(User.Identity.Name);
                var response = await _appointmentService.PlanAppointmentAsync(id, updatedBy);
                if (!response.Success)
                    return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }


        [HttpPost("{id}/custom-sms")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(Response<int>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> AddSms(int id) {
            try {
                var updatedBy = Convert.ToInt32(User.Identity.Name);
                var response = await _appointmentService.AddCustomSmsAsync(id, updatedBy);
                if (!response.Success)
                    return BadRequest(response);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(new Response(ex));
            }
        }
    }
}
