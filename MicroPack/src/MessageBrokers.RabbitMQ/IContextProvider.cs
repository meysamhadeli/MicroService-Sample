using System.Collections.Generic;

namespace MicroPack.MessageBrokers.RabbitMQ
{
    public interface IContextProvider
    {
        string HeaderName { get; }
        object Get(IDictionary<string, object> headers);
    }
}