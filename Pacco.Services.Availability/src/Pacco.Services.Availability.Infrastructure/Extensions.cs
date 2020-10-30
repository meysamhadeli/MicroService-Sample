using System;
using MicroPack.MessageBrokers.CQRS;
using MicroPack.MessageBrokers.RabbitMQ;
using MicroPack.MicroPack;
using MicroPack.Mongo;
using MicroPack.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application;
using Pacco.Services.Availability.Application.Commands;
using Pacco.Services.Availability.Application.Events.External;
using Pacco.Services.Availability.Application.Services;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Exceptions;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Infrastructure.Mongo.Repositories;
using Pacco.Services.Availability.Infrastructure.Services;

namespace Pacco.Services.Availability.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IResourcesRepository, ResourcesMongoRepository>();
            services.AddTransient<IMessageBroker, MessageBroker>();
            services.AddSingleton<IEventMapper, EventMapper>();
            services.AddSingleton<IEventProcessor, EventProcessor>();

            services.AddRabbitMq()
                .AddExceptionToMessageMapper<ExceptionToMessageMapper>();
            
            services.AddMongo()
                .AddMongoRepository<ResourceDocument, Guid>("resources");

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMicroPack()
                .UseErrorHandler();
            app.UseRabbitMq()
                .SubscribeCommand<AddResource>()
                .SubscribeCommand<ReserveResource>()
                .SubscribeEvent<CustomerCreated>();
            app.UsePublicContracts<ContractAttribute>();

            return app;
        }
        
    }
}