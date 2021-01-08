using System;
using System.Collections.Generic;
using System.Linq;
using MicroPack.Consul;
using MicroPack.Consul.Models;
using MicroPack.Fabio.Builders;
using MicroPack.Fabio.Http;
using MicroPack.Fabio.MessageHandlers;
using MicroPack.Http;
using MicroPack.MicroPack;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.Fabio
{
    public static class Extensions
    {
        private const string SectionName = "fabio";
        private const string RegistryName = "loadBalancing.fabio";

        public static IServiceCollection AddFabio(this IServiceCollection services, string sectionName = SectionName,
            string consulSectionName = "consul", string httpClientSectionName = "httpClient")
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }
            
            var fabioOptions = services.GetOptions<FabioOptions>(sectionName);
            var consulOptions = services.GetOptions<ConsulOptions>(consulSectionName);
            var httpClientOptions = services.GetOptions<HttpClientOptions>(httpClientSectionName);
            return services.AddFabio(fabioOptions, httpClientOptions,
                b => b.AddConsul(consulOptions, httpClientOptions));
        }

        public static IServiceCollection AddFabio(this IServiceCollection services,
            Func<IFabioOptionsBuilder, IFabioOptionsBuilder> buildOptions,
            Func<IConsulOptionsBuilder, IConsulOptionsBuilder> buildConsulOptions,
            HttpClientOptions httpClientOptions)
        {
            var fabioOptions = buildOptions(new FabioOptionsBuilder()).Build();
            return services.AddFabio(fabioOptions, httpClientOptions,
                b => b.AddConsul(buildConsulOptions, httpClientOptions));
        }

        public static IServiceCollection AddFabio(this IServiceCollection builder, FabioOptions fabioOptions,
            ConsulOptions consulOptions, HttpClientOptions httpClientOptions)
            => builder.AddFabio(fabioOptions, httpClientOptions, b => b.AddConsul(consulOptions, httpClientOptions));

        private static IServiceCollection AddFabio(this IServiceCollection services, FabioOptions fabioOptions,
            HttpClientOptions httpClientOptions, Action<IServiceCollection> registerConsul)
        {
            registerConsul(services);
            services.AddSingleton(fabioOptions);
            
            if (httpClientOptions.Type?.ToLowerInvariant() == "fabio")
            {
                services.AddTransient<FabioMessageHandler>();
                services.AddHttpClient<IFabioHttpClient, FabioHttpClient>("fabio-http")
                    .AddHttpMessageHandler<FabioMessageHandler>();


                services.RemoveInternalHttpClient();
                services.AddHttpClient<IHttpClient, FabioHttpClient>("fabio")
                    .AddHttpMessageHandler<FabioMessageHandler>();
            }

            using var serviceProvider = services.BuildServiceProvider();
            var registration = serviceProvider.GetService<ServiceRegistration>();
            var tags = GetFabioTags(registration.Name, fabioOptions.Service);
            if (registration.Tags is null)
            {
                registration.Tags = tags;
            }
            else
            {
                registration.Tags.AddRange(tags);
            }

            services.UpdateConsulRegistration(registration);

            return services;
        }

        public static void AddFabioHttpClient(this IConveyBuilder builder, string clientName, string serviceName)
            => builder.Services.AddHttpClient<IHttpClient, FabioHttpClient>(clientName)
                .AddHttpMessageHandler(c => new FabioMessageHandler(c.GetService<FabioOptions>(), serviceName));

        private static void UpdateConsulRegistration(this IServiceCollection services,
            ServiceRegistration registration)
        {
            var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(ServiceRegistration));
            services.Remove(serviceDescriptor);
            services.AddSingleton(registration);
        }

        private static List<string> GetFabioTags(string consulService, string fabioService)
        {
            var service = (string.IsNullOrWhiteSpace(fabioService) ? consulService : fabioService)
                .ToLowerInvariant();

            return new List<string> {$"urlprefix-/{service} strip=/{service}"};
        }
    }
}