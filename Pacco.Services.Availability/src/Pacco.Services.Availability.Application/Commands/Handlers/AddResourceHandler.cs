using System.Threading.Tasks;
using MicroPack.CQRS.Commands;
using MicroPack.RabbitMq;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Repositories;

namespace Pacco.Services.Availability.Application.Commands.Handlers
{
    public sealed class AddResourceHandler : ICommandHandler<AddResource>
    {
        private readonly IResourcesRepository _repository;

        public AddResourceHandler(IResourcesRepository repository)
        {
            _repository = repository;
        }
        
        public async Task HandleAsync(AddResource command, ICorrelationContext context)
        {
            var resource = await _repository.GetAsync(command.ResourceId);
            if (resource != null)
            {
                throw new ResourceAlreadyExistsException(command.ResourceId);
            }
            
            resource = Resource.Create(command.ResourceId, command.Tags);
            await _repository.AddAsync(resource);
        }
    }
}