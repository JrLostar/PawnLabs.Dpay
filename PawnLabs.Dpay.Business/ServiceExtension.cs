using Microsoft.Extensions.DependencyInjection;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Business.Service.Impl;

namespace PawnLabs.Dpay.Business
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();

            return services;
        }
    }
}
