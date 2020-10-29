using System.Threading.Tasks;
using MicroPack.CQRS.Events;

namespace Pacco.Services.Availability.Application.Events.External.Handlers
{
    public class CustomerCreatedHandler : IEventHandler<CustomerCreated>
    {
        public Task HandleAsync(CustomerCreated @event)
        {
            return Task.CompletedTask;
        }
    }
}