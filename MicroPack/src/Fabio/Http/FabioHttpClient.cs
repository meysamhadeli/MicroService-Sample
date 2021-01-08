using System.Net.Http;
using MicroPack.Http;

namespace MicroPack.Fabio.Http
{
    internal sealed class FabioHttpClient : MicroPackHttpClient, IFabioHttpClient
    {
        public FabioHttpClient(HttpClient client, HttpClientOptions options,
            ICorrelationContextFactory correlationContextFactory, ICorrelationIdFactory correlationIdFactory)
            : base(client, options, correlationContextFactory, correlationIdFactory)
        {
        }
    }
}