using System.Collections.Generic;
using System.Linq;
using Pacco.Services.Availability.Core.Events;

namespace Pacco.Services.Availability.Core.Entities
{
    public abstract class AggregateRoot
    {
        public List<IDomainEvent> Events { get; protected set; } = new List<IDomainEvent>();
        public int Version { get; protected set; }
        public AggregateId Id { get; protected set; }
        protected void AddEvent(IDomainEvent @event)
        {
            if (!Events.Any())
                Version++;
            
            Events.Add(@event);
        }
        
        public void ClearEvents() => Events.Clear();
    }
}