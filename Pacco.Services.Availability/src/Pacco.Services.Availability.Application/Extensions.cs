using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Events;
using MicroPack.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Pacco.Services.Availability.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
            => services
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher()
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher();
    }
}