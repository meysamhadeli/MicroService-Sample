using System.Threading.Tasks;
using MicroPack.CQRS.Queries;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Core.Repositories;

namespace Pacco.Services.Availability.Application.Queries.Handlers
{
    public sealed class GetResourceHandler : IQueryHandler<GetResource, ResourceDto>
    {
        private readonly IResourcesRepository _resourcesRepository;

        public GetResourceHandler(IResourcesRepository resourcesRepository)
        {
            _resourcesRepository = resourcesRepository;
        }
        public async Task<ResourceDto> HandleAsync(GetResource query)
        {

            var resource = await _resourcesRepository.GetAsync(new Core.Entities.AggregateId(query.ResourceId));

            return ResourceDto.FromEntity(resource);
        }
    }
}