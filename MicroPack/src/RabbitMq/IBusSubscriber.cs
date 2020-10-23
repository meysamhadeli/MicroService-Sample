using System;
using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Events;
using MicroPack.MicroPack;
using MicroPack.MicroPack.Types;

namespace MicroPack.RabbitMq
{
    public interface IBusSubscriber
    {
        IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
            Func<TCommand, MicroPackException, IRejectedEvent> onError = null)
            where TCommand : ICommand;

        IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
            Func<TEvent, MicroPackException, IRejectedEvent> onError = null) 
            where TEvent : IEvent;
    }
}
