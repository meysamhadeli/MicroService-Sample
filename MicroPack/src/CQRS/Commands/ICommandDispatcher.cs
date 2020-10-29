using System.Threading.Tasks;

namespace MicroPack.CQRS.Commands
{
    public interface ICommandDispatcher
    {
        Task SendAsync<T>(T command) where T : class, ICommand;
    }
}