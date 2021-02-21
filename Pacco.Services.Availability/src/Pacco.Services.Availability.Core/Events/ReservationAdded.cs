using System;
using MicroPack.Domain;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Events
{
    public class ReservationAdded : BaseDomainEvent<Resource, Guid>
    {
        public ReservationAdded()
        {
            
        }
        public Resource Resource { get; }
        public Reservation Reservation { get; }

        public ReservationAdded(Resource resource, Reservation reservation) : base(resource)
            => (Resource, Reservation) = (resource, reservation);
    }
}