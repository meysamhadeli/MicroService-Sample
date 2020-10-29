using System.Threading.Tasks;

namespace MicroPack.CQRS.Events
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}