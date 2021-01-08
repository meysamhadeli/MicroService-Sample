using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MicroPack.Consul.Models;

namespace MicroPack.Consul
{
    public interface IConsulService
    {
        Task<HttpResponseMessage> RegisterServiceAsync(ServiceRegistration registration);
        Task<HttpResponseMessage> DeregisterServiceAsync(string id);
        Task<IDictionary<string, ServiceAgent>> GetServiceAgentsAsync(string service = null);
    }
}