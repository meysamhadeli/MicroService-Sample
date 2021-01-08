using MicroPack.MessageBrokers.Outbox.EntityFramework.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.MessageBrokers.Outbox.EntityFramework
{
    public static class Extensions
    {
        public static IMessageOutboxConfigurator AddEntityFramework<T>(this IMessageOutboxConfigurator configurator)
            where T : DbContext
        {
            var services = configurator.Services;

            services.AddDbContext<T>();
            services.AddTransient<IMessageOutbox, EntityFrameworkMessageOutbox<T>>();
            services.AddTransient<IMessageOutboxAccessor, EntityFrameworkMessageOutbox<T>>();

            return configurator;
        }
    }
}