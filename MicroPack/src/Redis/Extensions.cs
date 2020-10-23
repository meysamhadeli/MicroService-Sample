using MicroPack.MicroPack;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.Redis
{
    public static class Extensions
    {
        private const string SectionName = "redis";
        public static IServiceCollection AddRedis(this IServiceCollection services, string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName)) sectionName = SectionName;

            var options = services.GetOptions<RedisOptions>(sectionName);
            return services.AddRedis(options);
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, RedisOptions options)
        {
            services.AddStackExchangeRedisCache(o =>
            {
                o.Configuration = options.ConnectionString;
                o.InstanceName = options.Instance;
            });

            return services;
        }
    }
}