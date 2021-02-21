using System;
using MicroPack.Domain;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Events
{
    public class ReservationReleased: BaseDomainEvent<Resource, Guid>
    {
        public ReservationReleased()
        {
            
        }
        public Resource Resource { get; }
        public Reservation Reservation { get; }

        public ReservationReleased(Resource resource, Reservation reservation) : base(resource)
            => (Resource, Reservation) = (resource, reservation);
    }
}