using System.Collections.Generic;
using System.Threading.Tasks;
using MicroPack.MessageBrokers.Outbox.Messages;

namespace MicroPack.MessageBrokers.Outbox
{
    public interface IMessageOutboxAccessor
    {
        Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync();
        Task ProcessAsync(OutboxMessage message);
        Task ProcessAsync(IEnumerable<OutboxMessage> outboxMessages);
    }
}