using System.Threading.Tasks;
using MicroPack.Consul.Models;

namespace MicroPack.Consul
{
    public interface IConsulServicesRegistry
    {
        Task<ServiceAgent> GetAsync(string name);
    }
}