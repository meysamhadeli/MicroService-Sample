using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using MicroPack.MessageBrokers.RabbitMQ.Clients;
using MicroPack.MessageBrokers.RabbitMQ.Contexts;
using MicroPack.MessageBrokers.RabbitMQ.Conventions;
using MicroPack.MessageBrokers.RabbitMQ.Initializers;
using MicroPack.MessageBrokers.RabbitMQ.Internals;
using MicroPack.MessageBrokers.RabbitMQ.Plugins;
using MicroPack.MessageBrokers.RabbitMQ.Publishers;
using MicroPack.MessageBrokers.RabbitMQ.Serializers;
using MicroPack.MessageBrokers.RabbitMQ.Subscribers;
using MicroPack.MicroPack;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
namespace MicroPack.MessageBrokers.RabbitMQ
{
    public static class Extensions
    {
        private const string SectionName = "rabbitmq";

        public static IServiceCollection AddRabbitMq(this IServiceCollection services, string sectionName = SectionName,
            Func<IRabbitMqPluginsRegistry, IRabbitMqPluginsRegistry> plugins = null,
            Action<ConnectionFactory> connectionFactoryConfigurator = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var options = services.GetOptions<RabbitMqOptions>(sectionName);
            services.AddSingleton(options);
            
            if (options.HostNames is null || !options.HostNames.Any())
            {
                throw new ArgumentException("RabbitMQ hostnames are not specified.", nameof(options.HostNames));
            }


            ILogger<IRabbitMqClient> logger;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                logger = serviceProvider.GetService<ILogger<IRabbitMqClient>>();
            }

            services.AddSingleton<IContextProvider, ContextProvider>();
            services.AddSingleton<ICorrelationContextAccessor>(new CorrelationContextAccessor());
            services.AddSingleton<IMessagePropertiesAccessor>(new MessagePropertiesAccessor());
            services.AddSingleton<IConventionsBuilder, ConventionsBuilder>();
            services.AddSingleton<IConventionsProvider, ConventionsProvider>();
            services.AddSingleton<IConventionsRegistry, ConventionsRegistry>();
            services.AddSingleton<IRabbitMqSerializer, NewtonsoftJsonRabbitMqSerializer>();
            services.AddSingleton<IRabbitMqClient, RabbitMqClient>();
            services.AddSingleton<IBusPublisher, RabbitMqPublisher>();
            services.AddSingleton<IBusSubscriber, RabbitMqSubscriber>();
            services.AddTransient<RabbitMqExchangeInitializer>();
            services.AddHostedService<RabbitMqHostedService>();

            var pluginsRegistry = new RabbitMqPluginsRegistry();
            services.AddSingleton<IRabbitMqPluginsRegistryAccessor>(pluginsRegistry);
            services.AddSingleton<IRabbitMqPluginsExecutor, RabbitMqPluginsExecutor>();
            plugins?.Invoke(pluginsRegistry);

            var connectionFactory = new ConnectionFactory
            {
                Port = options.Port,
                VirtualHost = options.VirtualHost,
                UserName = options.Username,
                Password = options.Password,
                RequestedHeartbeat = options.RequestedHeartbeat,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                SocketReadTimeout = options.SocketReadTimeout,
                SocketWriteTimeout = options.SocketWriteTimeout,
                RequestedChannelMax = options.RequestedChannelMax,
                RequestedFrameMax = options.RequestedFrameMax,
                UseBackgroundThreadsForIO = options.UseBackgroundThreadsForIO,
                DispatchConsumersAsync = true,
                ContinuationTimeout = options.ContinuationTimeout,
                HandshakeContinuationTimeout = options.HandshakeContinuationTimeout,
                NetworkRecoveryInterval = options.NetworkRecoveryInterval,
                Ssl = options.Ssl is null
                    ? new SslOption()
                    : new SslOption(options.Ssl.ServerName, options.Ssl.CertificatePath, options.Ssl.Enabled)
            };
            ConfigureSsl(connectionFactory, options, logger);
            connectionFactoryConfigurator?.Invoke(connectionFactory);

            logger.LogDebug($"Connecting to RabbitMQ: '{string.Join(", ", options.HostNames)}'...");
            var connection = connectionFactory.CreateConnection(options.HostNames.ToList(), options.ConnectionName);
            logger.LogDebug($"Connected to RabbitMQ: '{string.Join(", ", options.HostNames)}'.");
            services.AddSingleton(connection);
            services.BuildServiceProvider().AddInitializer<RabbitMqExchangeInitializer>();

            ((IRabbitMqPluginsRegistryAccessor) pluginsRegistry).Get().ToList().ForEach(p =>
                services.AddTransient(p.PluginType));

            return services;
        }

        private static void ConfigureSsl(ConnectionFactory connectionFactory, RabbitMqOptions options,
            ILogger<IRabbitMqClient> logger)
        {
            if (options.Ssl is null || string.IsNullOrWhiteSpace(options.Ssl.ServerName))
            {
                connectionFactory.Ssl = new SslOption();
                return;
            }

            connectionFactory.Ssl = new SslOption(options.Ssl.ServerName, options.Ssl.CertificatePath,
                options.Ssl.Enabled);

            logger.LogDebug($"RabbitMQ SSL is: {(options.Ssl.Enabled ? "enabled" : "disabled")}, " +
                            $"server: '{options.Ssl.ServerName}', client certificate: '{options.Ssl.CertificatePath}', " +
                            $"CA certificate: '{options.Ssl.CaCertificatePath}'.");

            if (string.IsNullOrWhiteSpace(options.Ssl.CaCertificatePath))
            {
                return;
            }

            connectionFactory.Ssl.CertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    return true;
                }

                if (chain is null)
                {
                    return false;
                }

                chain = new X509Chain();
                var certificate2 = new X509Certificate2(certificate);
                var signerCertificate2 = new X509Certificate2(options.Ssl.CaCertificatePath);
                chain.ChainPolicy.ExtraStore.Add(signerCertificate2);
                chain.Build(certificate2);
                var ignoredStatuses = Enumerable.Empty<X509ChainStatusFlags>();
                if (options.Ssl.X509IgnoredStatuses?.Any() is true)
                {
                    logger.LogDebug("Ignored X509 certificate chain statuses: " +
                                    $"{string.Join(", ", options.Ssl.X509IgnoredStatuses)}.");
                    ignoredStatuses  = options.Ssl.X509IgnoredStatuses
                        .Select(s => (X509ChainStatusFlags)Enum.Parse(typeof(X509ChainStatusFlags), s));
                }

                var statuses = chain.ChainStatus.ToList();
                logger.LogDebug("Received X509 certificate chain statuses: " +
                                $"{string.Join(", ", statuses.Select(x => x.Status))}");

                var isValid = statuses.All(chainStatus => chainStatus.Status == X509ChainStatusFlags.NoError
                                                          || ignoredStatuses.Contains(chainStatus.Status));
                if (!isValid)
                {
                    logger.LogError(string.Join(Environment.NewLine,
                        statuses.Select(s => $"{s.Status} - {s.StatusInformation}")));
                }

                return isValid;
            };
        }

        public static IServiceCollection AddExceptionToMessageMapper<T>(this IServiceCollection services)
            where T : class, IExceptionToMessageMapper
        {
            services.AddSingleton<IExceptionToMessageMapper, T>();

            return services;
        }

        public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
            => new RabbitMqSubscriber(app.ApplicationServices);
    }
}