using System;
using System.Threading.Tasks;
using MicroPack.Http;
using Pacco.Services.Availability.Application.DTO;
using Pacco.Services.Availability.Application.Services.Clients;

namespace Pacco.Services.Availability.Infrastructure.Services.Clients
{
    public class CustomersServiceClient: ICustomersServiceClient
    {
        private readonly IHttpClient _client;
        private readonly string _url;

        public CustomersServiceClient(IHttpClient client, HttpClientOptions options)
        {
            _client = client;
            _url = options.Services["customers"];
        }
        public Task<CustomerStateDto> GetStateAsync(Guid id)
            => _client.GetAsync<CustomerStateDto>($"{_url}/customers/{id}/state");
    }
}