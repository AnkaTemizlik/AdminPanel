using AutoMapper;
using DNA.API.PluginTemplate.Exceptions;
using DNA.API.PluginTemplate.Models;
using DNA.API.PluginTemplate.Resources;
using DNA.API.PluginTemplate.Services;
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
using System;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DNA.API.PluginTemplate.Controllers {
    [Route("api/invoice")]
    [Produces("application/json")]
    [Authorize]
    [ApiController]
    public class InvoiceController : ControllerBase {

        private readonly ILogger<InvoiceController> _logger;
        private readonly IProcessService _processService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValuerService _valuerService;
        private readonly ITranslationService _translationService;
        private readonly IMapper _mapper;
        private readonly IInvoiceService _invoiceService;
        private readonly IServiceProvider _services;

        public InvoiceController(IConfiguration configuration, ILogger<InvoiceController> logger, IMapper mapper, IInvoiceService invoiceService, IServiceProvider services, IProcessService processService, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IValuerService valuerService, ITranslationService translationService) {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
            _invoiceService = invoiceService;
            _services = services;
            _processService = processService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _valuerService = valuerService;
            _translationService = translationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Response<Invoice>), 200)]
        [ProducesResponseType(typeof(Response), 400)]
        public async Task<IActionResult> PostAsync([FromBody] InvoiceResource resource) {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                if (resource == null)
                    throw new Alert(AlertCodes.ValueCanNotBeEmpty, ("Field", "Invoice"));

                _logger.LogInformation(AlertCodes.ControllerIncomingData, ("action", "api/invoice"), ("resource", JsonConvert.SerializeObject(resource)));

                if (string.IsNullOrWhiteSpace(resource.InvoiceCode))
                    throw new Alert(AlertCodes.ValueCanNotBeEmpty, ("Field", "Invoice.InvoiceCode"));

                Response response = null;

                var invoice = _mapper.Map<InvoiceResource, Invoice>(resource);

                var scope = _services.CreateScope();
                var visitManager = scope.ServiceProvider.GetRequiredService<IScopedVisitManager>();
                visitManager.ResponseHandler += (s, e) => {
                    response = e;
                };

                await visitManager.DoWork(invoice);

                if (response == null)
                    throw new Alert(AlertCodes.NoResponseError);

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
