using System;

namespace Pacco.Services.Availability.Core.Exceptions
{
    public class CannotExpropriateReservationException : DomainException
    {
        public Guid ResourceId { get; set; }
        public DateTime DateTime { get; set; }
        public CannotExpropriateReservationException(Guid resourceId, DateTime dateTime):base($"Cannot expropriate reservation with id'{resourceId}' reservation at {dateTime}")
        {
            ResourceId = resourceId;
            DateTime = dateTime;
            ResourceId = resourceId;
        }
    }
}