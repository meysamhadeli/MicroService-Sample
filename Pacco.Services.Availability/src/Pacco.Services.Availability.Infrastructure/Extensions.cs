using System;
using Convey;
using MicroPack.MicroPack;
using MicroPack.Mongo;
using MicroPack.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Availability.Core.Entities;
using Pacco.Services.Availability.Core.Repositories;
using Pacco.Services.Availability.Infrastructure.Exceptions;
using Pacco.Services.Availability.Infrastructure.Mongo.Repositories;


namespace Pacco.Services.Availability.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddErrorHandler<ExceptionToResponseMapper>();
            services.AddTransient<IResourcesRepository, ResourcesMongoRepository>();

            services.AddMongo()
                .AddMongoRepository<Resource, Guid>("resources");

            return services;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseMicroPack()
                .UseErrorHandler();

            return app;
        }
    }
}