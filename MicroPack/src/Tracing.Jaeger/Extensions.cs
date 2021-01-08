using System;
using System.Threading;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using MicroPack.MicroPack;
using MicroPack.Tracing.Jaeger.Builders;
using MicroPack.Tracing.Jaeger.Tracers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Contrib.NetCore.Configuration;
using OpenTracing.Util;

namespace MicroPack.Tracing.Jaeger
{
    public static class Extensions
    {
        private static int _initialized;
        private const string SectionName = "jaeger";
        private const string RegistryName = "tracing.jaeger";

        public static IServiceCollection AddJaeger(this IServiceCollection services, string sectionName = SectionName,
            Action<IOpenTracingBuilder> openTracingBuilder = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var options = services.GetOptions<JaegerOptions>(sectionName);
            return services.AddJaeger(options, sectionName, openTracingBuilder);
        }

        public static IServiceCollection AddJaeger(this IServiceCollection services,
            Func<IJaegerOptionsBuilder, IJaegerOptionsBuilder> buildOptions,
            string sectionName = SectionName,
            Action<IOpenTracingBuilder> openTracingBuilder = null)
        {
            var options = buildOptions(new JaegerOptionsBuilder()).Build();
            return services.AddJaeger(options, sectionName, openTracingBuilder);
        }

        public static IServiceCollection AddJaeger(this IServiceCollection services, JaegerOptions options,
            string sectionName = SectionName, Action<IOpenTracingBuilder> openTracingBuilder = null)
        {
            if (Interlocked.Exchange(ref _initialized, 1) == 1)
            {
                return services;
            }

            services.AddSingleton(options);
            if (!options.Enabled)
            {
                var defaultTracer = ConveyDefaultTracer.Create();
                services.AddSingleton(defaultTracer);
                return services;
            }

            if (options.ExcludePaths is {})
            {
                services.Configure<AspNetCoreDiagnosticOptions>(o =>
                {
                    foreach (var path in options.ExcludePaths)
                    {
                        o.Hosting.IgnorePatterns.Add(x => x.Request.Path == path);
                    }
                });
            }

            services.AddOpenTracing(x => openTracingBuilder?.Invoke(x));

            services.AddSingleton<ITracer>(sp =>
            {
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var maxPacketSize = options.MaxPacketSize <= 0 ? 64967 : options.MaxPacketSize;
                var senderType = string.IsNullOrWhiteSpace(options.Sender) ? "udp" : options.Sender?.ToLowerInvariant();
                ISender sender = senderType switch
                {
                    "http" => BuildHttpSender(options.HttpSender),
                    "udp" => new UdpSender(options.UdpHost, options.UdpPort, maxPacketSize),
                    _ => throw new Exception($"Invalid Jaeger sender type: '{senderType}'.")
                };
                
                var reporter = new RemoteReporter.Builder()
                    .WithSender(sender)
                    .WithLoggerFactory(loggerFactory)
                    .Build();

                var sampler = GetSampler(options);

                var tracer = new Tracer.Builder(options.ServiceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithReporter(reporter)
                    .WithSampler(sampler)
                    .Build();

                GlobalTracer.Register(tracer);

                return tracer;
            });

            return services;
        }

        private static HttpSender BuildHttpSender(JaegerOptions.HttpSenderOptions options)
        {
            if (options is null)
            {
                throw new Exception("Missing Jaeger HTTP sender options.");
            }

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                throw new Exception("Missing Jaeger HTTP sender endpoint.");
            }
            
            var builder = new HttpSender.Builder(options.Endpoint);
            if (options.MaxPacketSize > 0)
            {
                builder = builder.WithMaxPacketSize(options.MaxPacketSize);
            }

            if (!string.IsNullOrWhiteSpace(options.AuthToken))
            {
                builder = builder.WithAuth(options.AuthToken);
            }
            
            if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
            {
                builder = builder.WithAuth(options.Username, options.Password);
            }
            
            if (!string.IsNullOrWhiteSpace(options.UserAgent))
            {
                builder = builder.WithUserAgent(options.Username);
            }

            return builder.Build();
        }

        public static IApplicationBuilder UseJaeger(this IApplicationBuilder app)
        {
            JaegerOptions options;
            using (var scope = app.ApplicationServices.CreateScope())
            {
                options = scope.ServiceProvider.GetService<JaegerOptions>();
            }

            return app;
        }

        private static ISampler GetSampler(JaegerOptions options)
        {
            switch (options.Sampler)
            {
                case "const": return new ConstSampler(true);
                case "rate": return new RateLimitingSampler(options.MaxTracesPerSecond);
                case "probabilistic": return new ProbabilisticSampler(options.SamplingRate);
                default: return new ConstSampler(true);
            }
        }
    }
}