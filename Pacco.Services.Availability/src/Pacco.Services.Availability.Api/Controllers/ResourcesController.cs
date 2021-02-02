using System.Threading.Tasks;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Queries;
using System.Collections.Generic;
using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Queries;
using MicroPack.MicroPack.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Pacco.Services.Availability.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly AppOptions _appOptions;

        public ResourcesController(IQueryDispatcher queryDispatcher,
            ICommandDispatcher commandDispatcher,
            IOptions<AppOptions> appOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            _httpContextAccessor = httpContextAccessor;
            _appOptions = appOptions.Value;
        }

        [HttpGet(nameof(Ping))]
        public Task Ping()
        {
            _httpContextAccessor.HttpContext.Response.WriteAsync(_appOptions.Name);
            return Task.CompletedTask;
        }


        [HttpGet("{resourceId}")]
        public async Task<ActionResult<ResourceDto>> Get([FromRoute] GetResource query)
        {
            var resource = await _queryDispatcher.QueryAsync<ResourceDto>(query);
            if (resource is null) return NotFound();
            return resource;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            await Task.Delay(1000);
            return "meysam";
        }

        [HttpPost(nameof(Find))]
        public async Task<IEnumerable<ResourceDto>> Find(GetResources query)
        {
            var resources = await _queryDispatcher.QueryAsync(query);

            return resources;
        }

        [HttpPost]
        public async Task<ActionResult> Post(AddResource command)
        {
            await _commandDispatcher.SendAsync(command);
            return Ok();
        }

        [HttpPost(nameof(ReserveResource))]
        public async Task<ActionResult> ReserveResource(ReserveResource command)
        {
            await _commandDispatcher.SendAsync(command);
            // return CreatedAtAction($"api/resources/ReserveResource/{command.ResourceId}",null);
            return Ok();
        }
    }
}