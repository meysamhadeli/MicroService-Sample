using System.Threading.Tasks;
using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Events;
using Pacco.Services.Availability.Application.Commands;

namespace Pacco.Services.Availability.Application.Events.External.Handlers
{
    public class VehicleDeletedHandler : IEventHandler<VehicleDeleted>
    {
        private readonly ICommandDispatcher _dispatcher;

        public VehicleDeletedHandler(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task HandleAsync(VehicleDeleted @event) => _dispatcher.SendAsync(new DeleteResource(@event.VehicleId));
    }
}