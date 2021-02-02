using System;
using System.Security.Cryptography.X509Certificates;
using MicroPack.MicroPack;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MicroPack.WebApi.Security
{
    public static class Extensions
    {
        private const string SectionName = "security";
        private const string RegistryName = "security";

        public static IServiceCollection AddCertificateAuthentication(this IServiceCollection services,
            string sectionName = SectionName, Type permissionValidatorType = null)
        {
            var options = services.GetOptions<SecurityOptions>(sectionName);
            services.AddSingleton(options);
          
            if (options.Certificate is null || !options.Certificate.Enabled)
            {
                return services;
            }

            if (permissionValidatorType is {})
            {
                services.AddSingleton(typeof(ICertificatePermissionValidator), permissionValidatorType);
            }
            else
            {
                services.AddSingleton<ICertificatePermissionValidator, DefaultCertificatePermissionValidator>();
            }
            
            services.AddSingleton<CertificateMiddleware>();
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate();
            services.AddCertificateForwarding(c =>
            {
                c.CertificateHeader = options.Certificate.GetHeaderName();
                c.HeaderConverter = headerValue =>
                    string.IsNullOrWhiteSpace(headerValue)
                        ? null
                        : new X509Certificate2(StringToByteArray(headerValue));
            });

            return services;
        }

        public static IApplicationBuilder UseCertificateAuthentications(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<SecurityOptions>();
            if (options.Certificate is null || !options.Certificate.Enabled)
            {
                return app;
            }

            app.UseCertificateForwarding();
            app.UseMiddleware<CertificateMiddleware>();

            return app;
        }

        private static byte[] StringToByteArray(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}