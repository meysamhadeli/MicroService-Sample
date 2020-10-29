using System;
using MicroPack.MicroPack;
using MicroPack.Redis.Builders;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MicroPack.Redis
{
    public static class Extensions
    {
        private const string SectionName = "redis";

        public static IServiceCollection AddRedis(this IServiceCollection services, string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var options = services.GetOptions<RedisOptions>(sectionName);
            return services.AddRedis(options);
        }

        public static IServiceCollection AddRedis(this IServiceCollection services,
            Func<IRedisOptionsBuilder, IRedisOptionsBuilder> buildOptions)
        {
            var options = buildOptions(new RedisOptionsBuilder()).Build();
            return services.AddRedis(options);
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, RedisOptions options)
        {
            
            services
                .AddSingleton(options)
                .AddTransient(sp => sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase(options.Database))
                .AddStackExchangeRedisCache(o =>
                {
                    o.Configuration = options.ConnectionString;
                    o.InstanceName = options.Instance;
                });

            return services;
        }
    }
}