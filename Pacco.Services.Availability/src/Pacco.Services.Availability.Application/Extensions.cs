using System.Runtime.CompilerServices;
using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Events;
using MicroPack.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;


[assembly: InternalsVisibleTo("Pacco.Services.Availability.Tests.Unit")]
namespace Pacco.Services.Availability.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
            => services
                .AddCommandHandlers()
                .AddEventHandlers()
                .AddQueryHandlers()
                .AddInMemoryCommandDispatcher()
                .AddInMemoryEventDispatcher()
                .AddInMemoryQueryDispatcher();
    }
}