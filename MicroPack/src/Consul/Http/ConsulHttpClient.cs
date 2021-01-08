using System.Net.Http;
using MicroPack.Http;

namespace MicroPack.Consul.Http
{
    internal sealed class ConsulHttpClient : MicroPackHttpClient, IConsulHttpClient
    {
        public ConsulHttpClient(HttpClient client, HttpClientOptions options,
            ICorrelationContextFactory correlationContextFactory, ICorrelationIdFactory correlationIdFactory)
            : base(client, options, correlationContextFactory, correlationIdFactory)
        {
        }
    }
}