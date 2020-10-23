using System.Threading.Tasks;
using MicroPack.RabbitMq;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.CQRS.Commands.Dispatchers
{
    internal sealed class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public CommandDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task SendAsync<T>(T command,ICorrelationContext context) where T : class, ICommand
        {
            using var scope = _serviceFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
            await handler.HandleAsync(command,context);
        }
    }
}