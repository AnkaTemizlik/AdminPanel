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

    [Route("api/notification")]
    [Produces("application/json")]
#if !DEBUG
    [Authorize] 
#endif
    [ApiController]
    public class NotificationController : ControllerBase {

        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, IMapper mapper, ILogger<NotificationController> logger) {
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response<QueryResult<Notification>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] QueryResource resource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var query = _mapper.Map<Query>(resource);
                query.CurrentUserId = Convert.ToInt32(User.Identity.Name);
                var result = await _notificationService.ListAsync(query);
                return Ok(new Response(result));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GeneralError, ex);
                return BadRequest(new Response(alert));
            }
        }

        [HttpPut("mark-as-read/{id}")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkAsRead(int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var result = await _notificationService.MarkAsReadOrUnreadAsync(id);
                return Ok(new Response<bool>(result));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GeneralError, ex);
                return BadRequest(new Response(alert));
            }
        }

        [HttpPost("delete-all-read")]
        [ProducesResponseType(typeof(Response<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAllRead() {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var result = await _notificationService.DeleteAllReadAsync(Convert.ToInt32(User.Identity.Name));
                return Ok(new Response<bool>(result));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.GeneralError, ex);
                return BadRequest(new Response(alert));
            }
        }
    }
}