using System;
using System.Threading.Tasks;
using MicroPack.CQRS.Commands;
using MicroPack.EventStore;
using Pacco.Services.Availability.Application.Exceptions;
using Pacco.Services.Availability.Application.Services;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Repositories;

namespace Pacco.Services.Availability.Application.Commands.Handlers
{
    public class AddResourceHandler : ICommandHandler<AddResource>
    {
        private readonly IResourcesRepository _repository;
        private readonly IEventProcessor _eventProcessor;
        private readonly IEventsService<Resource, Guid> _eventsService;

        public AddResourceHandler(IResourcesRepository repository, IEventProcessor eventProcessor, IEventsService<Resource, Guid> eventsService)
        {
            _repository = repository;
            _eventProcessor = eventProcessor;
            _eventsService = eventsService;
        }
        
        public async Task HandleAsync(AddResource command)
        {
            if (await _repository.ExistsAsync(command.ResourceId))
            {
                throw new ResourceAlreadyExistsException(command.ResourceId);
            }
            
            var resource = Resource.Create(command.ResourceId, command.Tags);
            await _repository.AddAsync(resource);
                
            //sample for save and get data from event source
            await _eventsService.SaveAsync(resource);
            var currentRecource = await _eventsService.GetByIdAsync(resource.Id);
            
            await _eventProcessor.ProcessAsync(resource.Events);
        }
    }
}