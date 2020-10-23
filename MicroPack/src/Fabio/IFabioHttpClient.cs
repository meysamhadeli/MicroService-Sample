using System.Threading.Tasks;

namespace MicroPack.Fabio
{
    public interface IFabioHttpClient
    {
        Task<T> GetAsync<T>(string requestUri);
    }
}