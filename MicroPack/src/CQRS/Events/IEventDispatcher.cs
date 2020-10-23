using System.Threading.Tasks;
using MicroPack.RabbitMq;

namespace MicroPack.CQRS.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync<T>(T @event, ICorrelationContext context = null) where T : class, IEvent;
    }
}