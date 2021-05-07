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
    [Route("api/module")]
    [ApiController]
    [Authorize]
    public class ModuleController : ControllerBase {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ModuleController> _logger;
        private readonly IModuleService _service;
        private readonly IProcessService _processService;
        private readonly IMapper _mapper;

        public ModuleController(IConfiguration configuration, ILogger<ModuleController> logger, IModuleService moduleService, IProcessService processService, IMapper mapper) {
            _configuration = configuration;
            _logger = logger;
            _service = moduleService;
            _processService = processService;
            _mapper = mapper;
        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(QueryResultResponse<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProgramsByLicenseId([FromQuery] QueryResource queryResource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var query = _mapper.Map<Query>(queryResource);
                var queryResult = await _processService.QueryAsync<Product>(SqlQueries.SelectProducts);
                return Ok(new QueryResultResponse<Product>(new Response(queryResult)));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GeneralError, ex);
                return Ok(new Response(alert));
            }
        }

        [HttpPost("product")]
        [ProducesResponseType(typeof(Response<ProductResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProductResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {

                await Task.CompletedTask;

                if (resource == null)
                    throw new ArgumentNullException("LicenseFileCreateResource");

                return Ok(resource);
            }
            catch (Exception ex) {
                var alert = _logger.LogError(Alerts.GeneralError, ex);
                return BadRequest(new Response(alert));
            }
        }

        [HttpPut("product/{id}")]
        [ProducesResponseType(typeof(Response<ProductResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(int id, [FromBody] ProductResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {

                await Task.CompletedTask;

                if (resource == null)
                    throw new ArgumentNullException("LicenseFileCreateResource");

                return Ok(resource);
            }
            catch (Exception ex) {
                var alert = _logger.LogError(Alerts.GeneralError, ex);
                return BadRequest(new Response(alert));
            }
        }
    }
}
