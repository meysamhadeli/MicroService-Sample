using System;
using System.Linq;
using System.Threading.Tasks;
using MicroPack.MicroPack.Types;
using MicroPack.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.MicroPack
{
    public static class Extensions
    {
        private const string SectionName = "app";

        public static IServiceCollection AddMicroPack(this IServiceCollection services, string sectionName = SectionName)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            var options = services.GetOptions<AppOptions>(sectionName);
            services.AddMemoryCache();
            services.AddSingleton(options);
            services.AddSingleton<IServiceId, ServiceId>();
            services.AddSingleton<IStartupInitializer,StartupInitializer>();

            if (!options.DisplayBanner || string.IsNullOrWhiteSpace(options.Name))
            {
                return services;
            }

            var version = options.DisplayVersion ? $" {options.Version}" : string.Empty;
            Console.WriteLine(Figgle.FiggleFonts.Doom.Render($"{options.Name}{version}"));

            return services;
        }

        public static IApplicationBuilder UseMicroPack(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<Types.IStartupInitializer>();
                Task.Run(() => initializer.InitializeAsync()).GetAwaiter().GetResult();
            }

            return app;
        }
        
        public static void AddInitializer(this IServiceProvider serviceProvider,IInitializer initializer)
        {
            var startupInitializer = serviceProvider.GetService<IStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        }

        public static void AddInitializer<TInitializer>(this IServiceProvider serviceProvider) where TInitializer : IInitializer
        {
            var initializer = serviceProvider.GetService<TInitializer>();
            var startupInitializer = serviceProvider.GetService<IStartupInitializer>();
            startupInitializer.AddInitializer(initializer);
        }
        
        public static IApplicationBuilder UseInitializers(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IStartupInitializer>();
                Task.Run(() => initializer.InitializeAsync()).GetAwaiter().GetResult();
            }

            return app;
        }
        
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string sectionName)
            where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(sectionName).Bind(model);
            return model;
        }

        public static TModel GetOptions<TModel>(this IServiceCollection services, string settingsSectionName)
            where TModel : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            return configuration.GetOptions<TModel>(settingsSectionName);
        }
        
        public static string Underscore(this string value)
            => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLowerInvariant();
    }
}