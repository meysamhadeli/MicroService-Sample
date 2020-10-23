using System.Threading.Tasks;

namespace MicroPack.Consul
{
    public interface IConsulHttpClient
    {
        Task<T> GetAsync<T>(string requestUri);
    }
}

