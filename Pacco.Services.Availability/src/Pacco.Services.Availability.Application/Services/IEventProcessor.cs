using System.Collections.Generic;
using System.Threading.Tasks;
using MicroPack.Domain;

namespace Pacco.Services.Availability.Application.Services
{
    public interface IEventProcessor
    {
        Task ProcessAsync(IEnumerable<IDomainEvent> events);
    }
}