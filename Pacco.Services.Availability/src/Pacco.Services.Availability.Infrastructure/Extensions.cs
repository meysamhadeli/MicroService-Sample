using System;
using EventStore.ClientAPI;
using MicroPack.Consul;
using MicroPack.CQRS.Commands;
using MicroPack.CQRS.Events;
using MicroPack.EventStore;
using MicroPack.Http;
using MicroPack.MessageBrokers.CQRS;
using MicroPack.MessageBrokers.Outbox;
using MicroPack.MessageBrokers.Outbox.Mongo;
using MicroPack.MessageBrokers.RabbitMQ;
using MicroPack.Metrics.AppMetrics;
using MicroPack.MicroPack;
using MicroPack.MicroPack.Types;
using MicroPack.Mongo;
using MicroPack.Security;
using MicroPack.Tracing.Jaeger;
using MicroPack.Tracing.Jaeger.RabbitMQ;
using MicroPack.WebApi;
using MicroPack.WebApi.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Events.External;
using Pacco.Services.Availability.Application.Services;
using Pacco.Services.Availability.Application.Services.Clients;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Events;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Decorators;
using Pacco.Services.Availability.Infrastructure.Exceptions;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Infrastructure.Mongo.Repositories;
using Pacco.Services.Availability.Infrastructure.Services;
using Pacco.Services.Availability.Infrastructure.Services.Clients;

namespace Pacco.Services.Availability.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            services.Configure<AppOptions>(configuration.GetSection("app"));

            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IResourcesRepository, ResourcesMongoRepository>();
            services.AddTransient<IMessageBroker, MessageBroker>();
            services.AddSingleton<IEventMapper, EventMapper>();
            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.TryDecorate(typeof(IEventHandler<>), typeof(OutboxEventHandlerDecorator<>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(OutboxCommandHandlerDecorator<>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(JaegerCommandHandlerDecorator<>));
            services.TryDecorate(typeof(ICommandHandler<>), typeof(SecurityDecorator<>));
            services.AddTransient<ICustomersServiceClient, CustomersServiceClient>();
            services.AddInternalHttpClientM();
            services.AddConsul();
            services.AddCertificateAuthentication();
            services.AddSecurity();
            //.AddFabio();
            // services.AddHostedService<MetricsJob>();
            services.AddMetricsInternal();
            services.AddJaeger();

            services.AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
                .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
                .AddMessageOutbox(o => o.AddMongo());

            services.AddMongo()
                .AddMongoRepository<ResourceDocument, Guid>("resources");

            services.AddEventStore()
                .AddEventsRepository<Resource, Guid>();
            
            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMicroPack()
                .UseErrorHandler();
            app.UseMetrics();
            app.UseJaeger();
            app.UseCertificateAuthentications();
            app.UseRabbitMq()
                .SubscribeCommand<AddResource>()
                .SubscribeCommand<ReserveResource>()
                .SubscribeEvent<CustomerCreated>();
            app.UsePublicContracts<ContractAttribute>();

            return app;
        }
    }
}