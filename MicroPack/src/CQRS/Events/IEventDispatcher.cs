using System.Threading.Tasks;

namespace MicroPack.CQRS.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync<T>(T @event) where T : class, IEvent;
    }
}