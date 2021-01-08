using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroPack.CQRS.Events;
using MicroPack.MessageBrokers;
using MicroPack.MessageBrokers.Outbox;
using MicroPack.MicroPack;
using Microsoft.Extensions.Logging;
using Pacco.Services.Availability.Application.Services;

namespace Pacco.Services.Availability.Infrastructure.Services
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IBusPublisher _busPublisher;
        private readonly ILogger<MessageBroker> _logger;
        private readonly IMessageOutbox _outbox;
        private readonly IMessagePropertiesAccessor _messagePropertiesAccessor;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public MessageBroker(IBusPublisher busPublisher, ILogger<MessageBroker> logger, IMessageOutbox outbox,
            IMessagePropertiesAccessor messagePropertiesAccessor, ICorrelationContextAccessor correlationContextAccessor)
        {
            _busPublisher = busPublisher;
            _logger = logger;
            _outbox = outbox;
            _messagePropertiesAccessor = messagePropertiesAccessor;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public Task PublishAsync(params IEvent[] events) => PublishAsync(events?.AsEnumerable());

        public async Task PublishAsync(IEnumerable<IEvent> events)
        {
            if (events is null)
            {
                return;
            }

            var correlationId = _messagePropertiesAccessor.MessageProperties?.CorrelationId;
            var correlationContext = _correlationContextAccessor.CorrelationContext;

            foreach (var @event in events)
            {
                if (@event is null)
                {
                    continue;
                }

                var messageId = Guid.NewGuid().ToString("N");
                _logger.LogTrace(
                    $"Publishing a integration event: {@event.GetType().Name.Underscore()} with Id: {messageId}.");

                if (_outbox.Enabled)
                {
                    await _outbox.SendAsync(@event, messageId: messageId, correlationId: correlationId, messageContext: correlationContext);
                    continue;
                }

                await _busPublisher.PublishAsync(@event, messageId, correlationId, messageContext: correlationContext);
            }
        }
    }
}