using System;
using MicroPack.CQRS.Events;

namespace Pacco.Services.Availability.Application.Events
{
    [Contract]
    public class ResourceAdded: IEvent
    {
        public Guid ResourceId { get; set; }

        public ResourceAdded(Guid resourceId)
        {
            ResourceId = resourceId;
        }
    }
}