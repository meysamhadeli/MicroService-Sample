using System.Threading.Tasks;
using MicroPack.RabbitMq;

namespace MicroPack.CQRS.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event,ICorrelationContext context);
    }
}