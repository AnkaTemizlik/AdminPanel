using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DNA.API.Infrastructure;
using DNA.API.Resources;
using DNA.Domain.Exceptions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Resources;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DNA.API.Controllers {

    [Route("api/log")]
    [Produces("application/json")]
    [Authorize]
    [ApiController]
    public class LogController : ControllerBase {

        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        private readonly ILogger<LogController> _logger;

        public LogController(ILogService logService, IMapper mapper, ILogger<LogController> logger) {
            _logService = logService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response<QueryResult<LogResource>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery]QueryResource queryResource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {

                var query = _mapper.Map<QueryResource, Query>(queryResource);

                var queryResult = await _logService.ListAsync(query);

                // var resource = _mapper.Map<QueryResult<Log>, QueryResultResource<LogResource>>(queryResult);

                return Ok(new Response(queryResult));

            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.LogListError, ex);
                return BadRequest(new Response(alert));
            }

        }

        [HttpGet("file")]
        [ProducesResponseType(typeof(Response<Dictionary<string, string>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLogFile() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            try {
                var queryResult = await _logService.ReadFileLogAsync();
                return Ok(new Response(queryResult));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Log file read error");
                return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Response<QueryResult<LogResource>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetById(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var log = await _logService.GetByIdAsync(id);
                var resource = _mapper.Map<Log, LogResource>(log);
                return Ok(new Response(log));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Single log reading error");
                return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
            }
        }
    }
}