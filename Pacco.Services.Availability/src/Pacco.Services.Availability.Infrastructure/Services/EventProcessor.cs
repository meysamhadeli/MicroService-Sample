using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pacco.Services.Availability.Application.Services;
using Pacco.Services.Availability.Core.Events;

namespace Pacco.Services.Availability.Infrastructure.Services
{
    public class EventProcessor: IEventProcessor
    {
        private readonly IEventMapper _eventMapper;
        private readonly IMessageBroker _messageBroker;
        private readonly ILogger<EventProcessor> _logger;

        public EventProcessor(IEventMapper eventMapper, IMessageBroker messageBroker, ILogger<EventProcessor> logger)
        {
            _eventMapper = eventMapper;
            _messageBroker = messageBroker;
            _logger = logger;
        }
        public async Task ProcessAsync(IEnumerable<IDomainEvent> events)
        {
            if (events is null)
            {
                return;
            }
            
            _logger.LogTrace("Processing domain events...");
            
            foreach (var @event in events)
            {
                //Handle domain event
            }
            _logger.LogTrace("Processing integration events...");
            var integrationEvents = _eventMapper.MapAll(events);
            await _messageBroker.PublishAsync(integrationEvents);
        }
    }
}