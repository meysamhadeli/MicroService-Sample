using System.Threading.Tasks;
using MongoDB.Driver;

namespace MicroPack.Mongo
{
    public interface IMongoSessionFactory
    {
        Task<IClientSessionHandle> CreateAsync();
    }
}