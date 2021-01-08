using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MicroPack.MicroPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace MicroPack.Http
{
    public static class Extensions
    {
        private const string SectionName = "httpClient";
        private const string RegistryName = "http.client";

        public static IServiceCollection AddInternalHttpClientM(this IServiceCollection services, string clientName = "convey",
            IEnumerable<string> maskedRequestUrlParts = null, string sectionName = SectionName,
            Action<IHttpClientBuilder> httpClientBuilder = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }
            
            if (string.IsNullOrWhiteSpace(clientName))
            {
                throw new ArgumentException("HTTP client name cannot be empty.", nameof(clientName));
            }

            var options = services.GetOptions<HttpClientOptions>(sectionName);
            if (maskedRequestUrlParts is {} && options.RequestMasking is {})
            {
                options.RequestMasking.UrlParts = maskedRequestUrlParts;
            }

            services.AddSingleton<ICorrelationContextFactory, EmptyCorrelationContextFactory>();
            services.AddSingleton<ICorrelationIdFactory, EmptyCorrelationIdFactory>();
            services.AddSingleton(options);
            var clientBuilder = services.AddHttpClient<IHttpClient, MicroPackHttpClient>(clientName);
            httpClientBuilder?.Invoke(clientBuilder);

            if (options.RequestMasking?.Enabled == true)
            {
                services.Replace(ServiceDescriptor
                    .Singleton<IHttpMessageHandlerBuilderFilter, MicroPackHttpLoggingFilter>());
            }

            return services;
        }

        [Description("This is a hack related to HttpClient issue: https://github.com/aspnet/AspNetCore/issues/13346")]
        public static void RemoveInternalHttpClient(this IServiceCollection services)
        {
            var registryType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .SingleOrDefault(t => t.Name == "HttpClientMappingRegistry");
            var registry = services.SingleOrDefault(s => s.ServiceType == registryType)?.ImplementationInstance;
            var registrations = registry?.GetType().GetProperty("TypedClientRegistrations");
            var clientRegistrations = registrations?.GetValue(registry) as IDictionary<Type, string>;
            clientRegistrations?.Remove(typeof(IHttpClient));
        }
    }
}