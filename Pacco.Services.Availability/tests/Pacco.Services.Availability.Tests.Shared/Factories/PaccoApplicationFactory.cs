using MicroPack.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Pacco.Services.Availability.Tests.Shared.Factories
{
    public class PaccoApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint: class
    {
        protected override IHostBuilder CreateHostBuilder() =>
            base.CreateHostBuilder().UseEnvironment("tests");
    }
}