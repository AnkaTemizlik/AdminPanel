using AutoMapper;
using DNA.Domain.Exceptions;
using DNA.Domain.Models;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Authorization;
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

        public AppointmentController(IConfiguration configuration, ILogger<AppointmentController> logger, IMapper mapper, IServiceProvider services, IProcessService processService, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IValuerService valuerService, ITranslationService translationService) {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _services = services;
            _processService = processService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _valuerService = valuerService;
            _translationService = translationService;
        }

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
    }
}
