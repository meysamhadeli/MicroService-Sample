using System.Collections.Generic;
using MicroPack.CQRS.Events;
using MicroPack.Domain;

namespace Pacco.Services.Availability.Application.Services
{
    public interface IEventMapper
    {
        IEvent Map(IDomainEvent @event);
        IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events);
    }
}