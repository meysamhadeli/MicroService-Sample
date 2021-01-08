using System;
using MicroPack.CQRS.Events;
using MicroPack.MessageBrokers;

namespace Pacco.Services.Availability.Application.Events.External
{
    [Message("customers")]
    public class CustomerCreated : IEvent
    {
        public Guid CustomerId { get; }

        public CustomerCreated(Guid customerId)
            => CustomerId = customerId;
    }
}