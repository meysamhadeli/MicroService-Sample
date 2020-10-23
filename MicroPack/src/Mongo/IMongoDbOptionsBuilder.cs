namespace MicroPack.Mongo
{
    public interface IMongoDbOptionsBuilder
    {
        IMongoDbOptionsBuilder WithConnectionString(string connectionString);
        IMongoDbOptionsBuilder WithDatabase(string database);
        IMongoDbOptionsBuilder WithSeed(bool seed);
        MongoDbOptions Build();
    }
}