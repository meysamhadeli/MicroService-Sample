using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroPack.CQRS.Queries;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Core.Repositories;

namespace Pacco.Services.Availability.Application.Queries.Handlers
{
    public class GetResourcesHandler : IQueryHandler<GetResources, IEnumerable<ResourceDto>>
    {
        private readonly IResourcesRepository _repository;

        public GetResourcesHandler(IResourcesRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<ResourceDto>> HandleAsync(GetResources query)
        {
            var allDocuments = await _repository.GetAllAsync();

            if (query.Tags is null || !query.Tags.Any())
            {

                return allDocuments.Select(d => ResourceDto.FromEntity(d));
            }

            allDocuments = query.MatchAllTags
                ? allDocuments.Where(d => query.Tags.All(t => d.Tags.Contains(t)))
                : allDocuments.Where(d => query.Tags.Any(t => d.Tags.Contains(t)));

            var resources = allDocuments.Select(d => ResourceDto.FromEntity(d));

            return resources;
        }
    }
}