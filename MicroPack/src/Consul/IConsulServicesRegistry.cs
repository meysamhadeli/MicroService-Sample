using System.Threading.Tasks;
using Consul;

namespace MicroPack.Consul
{
    public interface IConsulServicesRegistry
    {
        Task<AgentService> GetAsync(string name);
    }
}