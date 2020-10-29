using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Events
{
    public class ReservationAdded : IDomainEvent
    {
        public Resource Resource { get; private set; }
        public Reservation Reservation { get; private set; }

        public ReservationAdded(Resource resource, Reservation reservation)
        {
            this.Resource = resource;
            this.Reservation = reservation;
        }
    }
}