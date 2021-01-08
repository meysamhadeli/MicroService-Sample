using System;
using System.Threading.Tasks;
using MicroPack.CQRS.Events;
using MicroPack.MessageBrokers;
using MicroPack.MessageBrokers.Outbox;

namespace Pacco.Services.Availability.Infrastructure.Decorators
{
    public class OutboxEventHandlerDecorator<T> : IEventHandler<T> where T : class, IEvent
    {
        private readonly IEventHandler<T> _handler;
        private readonly IMessageOutbox _outbox;
        private readonly IMessagePropertiesAccessor _messagePropertiesAccessor;
        private readonly bool _enabled;
        private readonly string _messageId;

        public OutboxEventHandlerDecorator(IEventHandler<T> handler,
            IMessageOutbox outbox,
            IMessagePropertiesAccessor messagePropertiesAccessor)
        {
            _handler = handler;
            _outbox = outbox;
            _messagePropertiesAccessor = messagePropertiesAccessor;
            _enabled = outbox.Enabled;
            var messageProperties = messagePropertiesAccessor.MessageProperties;
            _messageId = string.IsNullOrWhiteSpace(messageProperties?.MessageId)
                ? Guid.NewGuid().ToString("N")
                : messageProperties.MessageId;
        }

        public Task HandleAsync(T @event) => _enabled
            ? _outbox.HandleAsync(_messageId, () => _handler.HandleAsync(@event))
            : _handler.HandleAsync(@event);
    }
}