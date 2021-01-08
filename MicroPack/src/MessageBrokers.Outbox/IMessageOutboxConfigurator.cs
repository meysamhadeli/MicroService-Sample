using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.MessageBrokers.Outbox
{
    public interface IMessageOutboxConfigurator
    {
        IServiceCollection Services { get; }
        OutboxOptions Options { get; }
    }
}