using System;
using System.Collections.Generic;
using System.Linq;
using Pacco.Services.Availability.Core.Events;
using Pacco.Services.Availability.Core.Exceptions;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Entities
{
    public class Resource : AggregateRoot
    {
        public List<string> Tags { get; set; } = new List<string>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        
        public Resource()
        {
        }

        public Resource(Guid id, IEnumerable<string> tags, IEnumerable<Reservation> reservations = null,
            int version = 0)
        {
            ValidateTags(tags);
            Id = id;
            Tags = tags?.ToList();
            Reservations = reservations?.ToList() ?? new List<Reservation>();
            Version = version;
        }

        private static void ValidateTags(IEnumerable<string> tags)
        {
            if (tags is null || !tags.Any())
            {
                throw new MissingResourceTagsException();
            }

            if (tags.Any(string.IsNullOrWhiteSpace))
            {
                throw new InvalidResourceTagsException();
            }
        }

        public static Resource Create(Guid id, IEnumerable<string> tags, IEnumerable<Reservation> reservations = null)
        {
            var resource = new Resource(id, tags, reservations);
            resource.AddEvent(new ResourceCreated(resource));
            return resource;
        }


        public void AddReservation(Reservation reservation)
        {
            var hasCollidingReservation = Reservations.Any(HasSameReservationdate);
            
            if (hasCollidingReservation)
            {
                var collidingReservation = Reservations.First(HasSameReservationdate);
            
                if (collidingReservation.Priority >= reservation.Priority)
                {
                    throw new CannotExpropriateReservationException(Id, reservation.DateTime);
                }
            
                Reservations.Remove(collidingReservation);
                AddEvent(new ReservationCanceled(this, collidingReservation));
            }
            
            Reservations.Add(reservation);
            AddEvent(new ReservationAdded(this, reservation));
            
            bool HasSameReservationdate(Reservation r) => r.DateTime == reservation.DateTime;
        }
    }
}