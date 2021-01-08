using System;
using MicroPack.CQRS.Events;

namespace Pacco.Services.Availability.Application.Events
{
    [Contract]
    public class ResourceReservationReleased : IEvent
    {
        public Guid ResourceId { get; }
        public DateTime DateTime { get; }

        public ResourceReservationReleased(Guid resourceId, DateTime dateTime)
            => (ResourceId, DateTime) = (resourceId, dateTime);
    }
}