using System;
using System.Collections.Generic;
using MicroPack.Domain;
using Newtonsoft.Json;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Events
{
    public class ResourceCreated : BaseDomainEvent<Resource, Guid>
    {
        public ResourceCreated()
        {
            
        }
        public ResourceCreated(Resource resource) : base(resource)
        {
            Id = resource.Id;
            Reservations = resource.Reservations;
            Tags = resource.Tags;
        }
        public Guid Id { get; private set; }
        public IEnumerable<Reservation> Reservations { get; private set;}
        public IEnumerable<string> Tags { get; private set;}
    }
}