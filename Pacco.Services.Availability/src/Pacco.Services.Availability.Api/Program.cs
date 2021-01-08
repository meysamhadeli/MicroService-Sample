using System.Threading.Tasks;
using MicroPack.Logging;
using MicroPack.Vault;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pacco.Services.Availability.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await CreateHostBuilder(args)
                .Build()
                .RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLogging()
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>());
    }
}
