using MicroPack.Security.Internals;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.Security
{
    public static class Extensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            services
                .AddSingleton<IEncryptor, Encryptor>()
                .AddSingleton<IHasher, Hasher>()
                .AddSingleton<ISigner, Signer>();

            return services;
        }
    }
}