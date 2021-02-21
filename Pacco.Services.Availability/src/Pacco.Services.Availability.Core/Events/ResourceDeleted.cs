using System;
using System.Collections.Generic;
using MicroPack.Domain;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.ValueObjects;

namespace Pacco.Services.Availability.Core.Events
{
    public class ResourceDeleted: BaseDomainEvent<Resource,Guid>
    {
        public Resource Resource { get; }

        public ResourceDeleted(Guid id, IEnumerable<string> tags, IEnumerable<Reservation> reservations) : base(new Resource(id, tags, reservations))
        {
            
        }
    }
}