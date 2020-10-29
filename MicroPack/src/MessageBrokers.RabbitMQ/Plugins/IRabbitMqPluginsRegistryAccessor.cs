using System.Collections.Generic;

namespace MicroPack.MessageBrokers.RabbitMQ.Plugins
{
    internal interface IRabbitMqPluginsRegistryAccessor
    {
        LinkedList<RabbitMqPluginChain> Get();
    }
}