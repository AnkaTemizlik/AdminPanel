using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DNA.API.Infrastructure;
using DNA.API.Resources;
using DNA.Domain.Exceptions;
using DNA.Domain.Models;
using DNA.Domain.Models.Queries;
using DNA.Domain.Services;
using DNA.Domain.Services.Communication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DNA.API.Controllers {

    [Route("api/entity")]
    [Produces("application/json")]
#if !DEBUG
    [Authorize] 
#endif
    [ApiController]
    public class EntityController : ControllerBase {

        private readonly IEntityService _entityService;
        private readonly IProcessService _processService;
        private readonly IMapper _mapper;
        private readonly ILogger<LogController> _logger;

        public EntityController(IEntityService entityService, IProcessService processService, IMapper mapper, ILogger<LogController> logger) {
            _entityService = entityService;
            this._processService = processService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Response<QueryResult<Model>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntities([FromQuery] EntityQueryResource query) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                query.Name = query.Name?.Trim('"');
                var entityQuery = _mapper.Map<EntityQuery>(query);
                entityQuery.CurrentUserId = Convert.ToInt32(User.Identity.Name);

                var queryResult = await _entityService.ListAsync(entityQuery);
                var i = queryResult.Items;
                return Ok(new Response(queryResult));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.EntityListError, ex, ("Query", JsonConvert.SerializeObject(query)));
                return BadRequest(new Response(alert));
            }
        }

        [HttpGet("{name}/{id}")]
        [ProducesResponseType(typeof(Response<Model>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityById(string name, string id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var result = await _entityService.GetByIdAsync(name, id);
                if (result == null)
                    return NotFound(new Response("Not found"));

                return Ok(new Response(result));
            }
            catch (Exception ex) {
                var alert = _logger.LogError(AlertCodes.EntityListError, ex, (name, id));
                return BadRequest(new Response(alert));
            }
        }

        [HttpPost("{name}")]
        [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InsertEntity(string name, [FromBody] dynamic entity) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var result = await _entityService.InsertAsync(name, JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(entity)));
                return Ok(new Response(result));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Entity insert error for ", name);
                return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
            }
        }

        [HttpPut("{name}/{id}")]
        [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEntity(string name, string id, [FromBody] dynamic entity) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            try {
                var result = await _entityService.UpdateAsync(name, id, JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(entity)));
                return Ok(new Response(result));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Entity update error for ", name);
                return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
            }
        }

        [HttpDelete("{name}/{id}")]
        [ProducesResponseType(typeof(Response<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEntity(string name, string id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var result = await _entityService.DeleteAsync(name, new { Id = id });
                return Ok(new Response(result));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Entity update error for ", name);
                return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
            }
        }

        //[HttpPost("list")]
        //[ProducesResponseType(typeof(Response<QueryResult<dynamic>>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetEntityList([FromQuery] EntityQueryResource query, [FromBody] ConditionCollection conditions) {
        //    if (!ModelState.IsValid) {
        //        return BadRequest(ModelState);
        //    }

        //    try {

        //        var entityQuery = _mapper.Map<EntityQueryResource, EntityQuery>(query);
        //        entityQuery.CurrentUserId = Convert.ToInt32(User.Identity.Name);
        //        //entityQuery.Filter = string.Join(" AND ", conditions.Select(_ => _.ToString()));
        //        entityQuery.Prepare(conditions);
        //        var queryResult = await _entityService.ListAsync(entityQuery);
        //        return Ok(new Response(queryResult));
        //    }
        //    catch (Exception ex) {
        //        _logger.LogError(ex, "Entity list error: Filter:{2}", query.Page, query.Take, query.Filter);
        //        return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
        //    }
        //}

        //[HttpPost("model")]
        //[ProducesResponseType(typeof(Response<dynamic>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetEntityModel([FromQuery] EntityQueryResource query, [FromBody] ConditionCollection conditions) {
        //    if (!ModelState.IsValid) {
        //        return BadRequest(ModelState);
        //    }

        //    try {
        //        var entityQuery = _mapper.Map<EntityQueryResource, EntityQuery>(query);
        //        entityQuery.CurrentUserId = Convert.ToInt32(User.Identity.Name);
        //        entityQuery.Prepare(conditions);
        //        var queryResult = await _entityService.GetEntityModelAsync(entityQuery);
        //        return Ok(new Response((object)(queryResult ?? new { })));
        //    }
        //    catch (Exception ex) {
        //        _logger.LogError(ex, "Entity list error: Filter:{2}", query.Page, query.Take, query.Filter);
        //        return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
        //    }
        //}

        [HttpGet("cards")]
        [ProducesResponseType(typeof(Response<Dictionary<string, IEnumerable<dynamic>>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCards([FromQuery(Name = "Names[]")] string[] names) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                var cardsQuery = _mapper.Map<CardQueryResource, CardsQuery>(new CardQueryResource() { Names = names });
                var queryResult = await _entityService.CardListAsync(cardsQuery);
                return Ok(new Response(queryResult));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Cards list error");
                return BadRequest(Errors.AddErrorToModelState("failure", ex.Message, ModelState));
            }

        }
    }
}
