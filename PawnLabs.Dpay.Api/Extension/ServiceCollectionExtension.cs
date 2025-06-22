using PawnLabs.Dpay.Core.Configuration;
using PawnLabs.Dpay.Core.Helper;
using PawnLabs.Dpay.Core.Helper.Impl;
using PawnLabs.Dpay.Core.Option;

namespace PawnLabs.Dpay.Api.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenConfiguration>(configuration.GetSection("TokenConfiguration"));
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.Configure<StellarConfiguration>(configuration.GetSection("StellarConfiguration"));
            services.Configure<ModalConfiguration>(configuration.GetSection("ModalConfiguration"));

            return services;
        }

        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddScoped<IMailHelper, MailHelper>();
            services.AddScoped<ISecurityHelper, SecurityHelper>();
            services.AddScoped<IConfigurationHelper, ConfigurationHelper>();
            services.AddScoped<ISocketHelper, SocketHelper>();

            return services;
        }
    }
}
