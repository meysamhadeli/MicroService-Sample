using System.Threading.Tasks;

namespace MicroPack.CQRS.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand command);
    }
}