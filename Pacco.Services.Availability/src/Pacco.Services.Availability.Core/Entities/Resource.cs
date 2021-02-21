using System;
using System.Collections.Generic;
using System.Linq;
using MicroPack.Domain;
using Pacco.Services.Availability.Core.Events;
using Pacco.Services.Availability.Core.Exceptions;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Entities
{
    public class Resource : BaseAggregateRoot<Resource, Guid>
    {
        public Resource()
        {
        }

        public HashSet<Reservation> Reservations { get; private set; }
        public HashSet<string> Tags { get; private set; }

        public Resource(Guid id, IEnumerable<string> tags, IEnumerable<Reservation> reservations = null) : base(id)
        {
            ValidateTags(tags);
            Id = id;
            Tags = tags.ToHashSet();
            Reservations = reservations?.ToHashSet() ?? new HashSet<Reservation>();
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
            var hasCollidingReservation = Reservations.Any(HasTheSameReservationDate);
            if (hasCollidingReservation)
            {
                var collidingReservation = Reservations.First(HasTheSameReservationDate);
                if (collidingReservation.Priority >= reservation.Priority)
                {
                    throw new CannotExpropriateReservationException(Id, reservation.DateTime.Date);
                }

                if (Reservations.Remove(collidingReservation))
                {
                    AddEvent(new ReservationCanceled(new Resource(Id, Tags, Reservations), collidingReservation));
                }
            }

            if (Reservations.Add(reservation))
            {
                AddEvent(new ReservationAdded(new Resource(Id, Tags, Reservations), reservation));
            }

            bool HasTheSameReservationDate(Reservation r) => r.DateTime.Date == reservation.DateTime.Date;
        }

        public void ReleaseReservation(Reservation reservation)
        {
            if (!Reservations.Remove(reservation))
            {
                return;
            }

            AddEvent(new ReservationReleased(new Resource(Id, Tags, Reservations), reservation));
        }

        public void Delete()
        {
            foreach (var reservation in Reservations)
            {
                AddEvent(new ReservationCanceled(new Resource(Id, Tags, Reservations), reservation));
            }

            AddEvent(new ResourceDeleted(Id, Tags, Reservations));
        }

        protected override void Apply(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case ResourceCreated r:
                    this.Id = r.Id;
                    this.Tags = r.Tags.ToHashSet();
                    break;
            }
        }
    }
}