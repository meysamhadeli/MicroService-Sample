using System.Threading.Tasks;
using MongoDB.Driver;

namespace MicroPack.Mongo
{
    public interface IMongoDbSeeder
    {
        Task SeedAsync(IMongoDatabase database);
    }
}