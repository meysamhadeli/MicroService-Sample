using System.Threading.Tasks;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Queries;
using System.Collections.Generic;
using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Pacco.Services.Availability.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public ResourcesController(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        [HttpGet("{resourceId}")]
        public async Task<ActionResult<ResourceDto>> Get([FromRoute] GetResource query)
        {
            var resource = await _queryDispatcher.QueryAsync<ResourceDto>(query);
            if (resource is null) return NotFound();

            return resource;
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
            return CreatedAtAction($"api/resources/{command.ResourceId}",null);
        }
        
        
        

        
      
    }
}