using MicroPack.MicroPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.MailKit
{
    public static class Extension
    {
        public static void AddMailKit(this ServiceCollection services, string sectionName = "mailkit")
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            var mailKitOptions = configuration.GetOptions<MailKitOptions>(sectionName);
            services.AddSingleton(mailKitOptions);
        }
    }
}