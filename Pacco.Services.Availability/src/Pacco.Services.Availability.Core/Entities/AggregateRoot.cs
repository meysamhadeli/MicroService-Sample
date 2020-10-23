using System.Collections.Generic;
using System.Linq;
using MicroPack.MicroPack.Types;
using Pacco.Services.Availability.Core.Events;

namespace Pacco.Services.Availability.Core.Entities
{
    public abstract class AggregateRoot<T>: IIdentifiable<T>
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IEnumerable<IDomainEvent> Events => _events;
        public T Id { get; protected set; }
        public int Version { get; protected set; }

        protected void AddEvent(IDomainEvent @event)
        {
            if (!_events.Any())
            {
                Version++;
            }

            _events.Add(@event);
        }

        public void ClearEvents() => _events.Clear();
    }
}