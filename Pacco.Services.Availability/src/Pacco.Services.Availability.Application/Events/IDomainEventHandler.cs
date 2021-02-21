using System.Threading.Tasks;

namespace Pacco.Services.Availability.Application.Events
{
    public interface IDomainEventHandler<in T> where T : class
    {
        Task HandleAsync(T @event);
    }
}