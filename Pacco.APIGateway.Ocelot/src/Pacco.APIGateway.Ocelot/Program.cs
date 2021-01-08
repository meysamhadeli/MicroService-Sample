using System.Threading.Tasks;
using MicroPack.Authentication;
using MicroPack.MessageBrokers.RabbitMQ;
using MicroPack.MicroPack;
using MicroPack.MicroPack.Types;
using MicroPack.Redis;
using MicroPack.Security;
using MicroPack.Tracing.Jaeger;
using MicroPack.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Pacco.APIGateway.Ocelot.Infrastructure;

namespace Pacco.APIGateway.Ocelot
{
    public class Program
    {
        public static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>());
    }
}

