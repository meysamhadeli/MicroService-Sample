using MicroPack.MessageBrokers.RabbitMQ;
using MicroPack.Tracing.Jaeger.RabbitMQ.Plugins;

namespace MicroPack.Tracing.Jaeger.RabbitMQ
{
    public static class Extensions
    {
        public static IRabbitMqPluginsRegistry AddJaegerRabbitMqPlugin(this IRabbitMqPluginsRegistry registry)
        {
            registry.Add<JaegerPlugin>();
            return registry;
        }
    }
}