using System.Threading.Tasks;
using MicroPack.RabbitMq;

namespace MicroPack.CQRS.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand @event,ICorrelationContext context);
    }
}