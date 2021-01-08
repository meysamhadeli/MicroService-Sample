using System;
using MicroPack.CQRS.Events;
using MicroPack.MessageBrokers;

namespace Pacco.Services.Availability.Application.Events.External
{
    [Message("vehicles")]
    public class VehicleDeleted : IEvent
    {
        public Guid VehicleId { get; }

        public VehicleDeleted(Guid vehicleId)
            => VehicleId = vehicleId;
    }
}