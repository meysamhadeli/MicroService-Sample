using System;
using MicroPack.MicroPack;
using MicroPack.MicroPack.Types;
using MicroPack.Mongo.Builders;
using MicroPack.Mongo.Factories;
using MicroPack.Mongo.Initializers;
using MicroPack.Mongo.Repositories;
using MicroPack.Mongo.Seeders;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace MicroPack.Mongo
{
    public static class Extensions
    {
        // Helpful when dealing with integration testing
        private static bool _conventionsRegistered;
        private const string SectionName = "mongo";

        public static IServiceCollection AddMongo(this IServiceCollection services, string sectionName = SectionName,
            Type seederType = null, bool registerConventions = true)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var mongoOptions = services.GetOptions<MongoDbOptions>(sectionName);
            return services.AddMongo(mongoOptions, seederType, registerConventions);
        }

        public static IServiceCollection AddMongo(this IServiceCollection services, Func<IMongoDbOptionsBuilder,
            IMongoDbOptionsBuilder> buildOptions, Type seederType = null, bool registerConventions = true)
        {
            var mongoOptions = buildOptions(new MongoDbOptionsBuilder()).Build();
            return services.AddMongo(mongoOptions, seederType, registerConventions);
        }

        public static IServiceCollection AddMongo(this IServiceCollection services, MongoDbOptions mongoOptions,
            Type seederType = null, bool registerConventions = true)
        {
            if (mongoOptions.SetRandomDatabaseSuffix)
            {
                var suffix = $"{Guid.NewGuid():N}";
                Console.WriteLine($"Setting a random MongoDB database suffix: '{suffix}'.");
                mongoOptions.Database = $"{mongoOptions.Database}_{suffix}";
            }

            services.AddSingleton(mongoOptions);
            services.AddSingleton<IMongoClient>(sp =>
            {
                var options = sp.GetService<MongoDbOptions>();
                return new MongoClient(options.ConnectionString);
            });
            services.AddTransient(sp =>
            {
                var options = sp.GetService<MongoDbOptions>();
                var client = sp.GetService<IMongoClient>();
                return client.GetDatabase(options.Database);
            });
            services.AddTransient<IMongoSessionFactory, MongoSessionFactory>();
            services.AddTransient<IMongoDbInitializer, MongoDbInitializer>();

            if (seederType is null)
            {
                services.AddTransient<IMongoDbSeeder, MongoDbSeeder>();
            }
            else
            {
                services.AddTransient(typeof(IMongoDbSeeder), seederType);
            }
            
            
            services.BuildServiceProvider().AddInitializer<IMongoDbInitializer>();
            
            if (registerConventions && !_conventionsRegistered)
            {
                RegisterConventions();
            }

            return services;
        }

        private static void RegisterConventions()
        {
            _conventionsRegistered = true;
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?),
                new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            ConventionRegistry.Register("convey", new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
            }, _ => true);
        }

        public static IServiceCollection AddMongoRepository<TEntity, TIdentifiable>(this IServiceCollection services,
            string collectionName)
            where TEntity : IIdentifiable<TIdentifiable>
        {
            services.AddTransient<IMongoRepository<TEntity, TIdentifiable>>(sp =>
            {
                var database = sp.GetService<IMongoDatabase>();
                return new MongoRepository<TEntity, TIdentifiable>(database, collectionName);
            });

            return services;
        }
    }
}