using System;
using MicroPack.MessageBrokers.CQRS;
using MicroPack.MessageBrokers.RabbitMQ;
using MicroPack.MicroPack;
using MicroPack.Mongo;
using MicroPack.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Application.Events.External;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Exceptions;
using Pacco.Services.Availability.Infrastructure.Mongo.Documents;
using Pacco.Services.Availability.Infrastructure.Mongo.Repositories;

namespace Pacco.Services.Availability.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IResourcesRepository, ResourcesMongoRepository>();
            services.AddRabbitMq();

            services.AddMongo()
                .AddMongoRepository<ResourceDocument, Guid>("resources");

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMicroPack()
                .UseErrorHandler();
            app.UseRabbitMq()
            .SubscribeEvent<CustomerCreated>();

            return app;
        }
    }
}