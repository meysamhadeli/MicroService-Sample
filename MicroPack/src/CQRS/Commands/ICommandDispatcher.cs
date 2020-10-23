using System.Threading.Tasks;
using MicroPack.RabbitMq;

namespace MicroPack.CQRS.Commands
{
    public interface ICommandDispatcher
    {
        Task SendAsync<T>(T command,ICorrelationContext context = null) where T : class, ICommand;
    }
}