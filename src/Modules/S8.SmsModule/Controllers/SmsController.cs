using AutoMapper;
using DNA.Domain.Exceptions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Resources;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using S8.SmsModule.Exceptions;
using S8.SmsModule.Models;
using S8.SmsModule.Resources;
using S8.SmsModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S8.SmsModule.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SmsController : ControllerBase {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsController> _logger;
        private readonly ISmsService _service;
        private readonly IProcessService _processService;
        private readonly IMapper _mapper;

        public SmsController(IConfiguration configuration, ILogger<SmsController> logger, ISmsService moduleService, IProcessService processService, IMapper mapper) {
            _configuration = configuration;
            _logger = logger;
            _service = moduleService;
            _processService = processService;
            _mapper = mapper;
        }

        [HttpPost("send/{config}")]
        [ProducesResponseType(typeof(QueryResultResponse<Sms>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendSms(string config, [FromBody] SmsResource smsResource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                //var query = _mapper.Map<Sms>(smsResource);
                //var queryResult = await _processService.QueryAsync<Sms>(SqlQueries.SelectProducts);
                await Task.CompletedTask;
                return Ok(new Response());
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GeneralError, ex);
                return Ok(new Response(alert));
            }
        }

    }
}
