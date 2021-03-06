using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DNA.API.Module.Exceptions;
using DNA.API.Module.Models;
using DNA.API.Module.Resources;
using DNA.API.Module.Services;
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

namespace DNA.API.Module.Controllers {

    [Route("api/[controller]")]
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

    }
}
