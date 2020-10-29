using System;

namespace MicroPack.MessageBrokers.RabbitMQ
{
    public interface IExceptionToMessageMapper
    {
        object Map(Exception exception, object message);
    }
}