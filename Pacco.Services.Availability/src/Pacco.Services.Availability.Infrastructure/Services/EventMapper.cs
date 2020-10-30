using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MicroPack.CQRS.Events;
using Pacco.Services.Availability.Application.Events;
using Pacco.Services.Availability.Application.Services;
using Pacco.Services.Availability.Core.Events;

namespace Pacco.Services.Availability.Infrastructure.Services
{
    public class EventMapper : IEventMapper
    {
        public IEvent Map(IDomainEvent @event)
            => @event switch
            {
                ResourceCreated e => new ResourceAdded(e.Resource.Id),
                ReservationAdded e => new ResourceReserved(e.Resource.Id, e.Reservation.DateTime),
                ReservationCanceled e => new ResourceReservationCanceled(e.Resource.Id, e.Reservation.DateTime),
                _ => null
            };

        public IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events)
            => events?.Select(Map);
    }
}