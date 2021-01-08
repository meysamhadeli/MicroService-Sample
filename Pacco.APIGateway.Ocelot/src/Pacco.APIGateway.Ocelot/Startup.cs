using Convey;
using Convey.Auth;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Types;
using Convey.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Pacco.APIGateway.Ocelot.Infrastructure;

namespace Pacco.APIGateway.Ocelot
{
    public class Startup
        {
            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public IConfiguration Configuration { get; }

            // This method gets called by the runtime. Use this method to add services to the container.
            public void ConfigureServices(IServiceCollection services)
            {
              

                services
                    .AddControllers()
                    .AddNewtonsoftJson();

              
            }

            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                // app.UseConvey();
                //
                // app.UseAuthentication();
              
            }
            
            
        }
}