using Microsoft.Extensions.DependencyInjection;
using PawnLabs.Dpay.Data.Repository;
using PawnLabs.Dpay.Data.Repository.Impl;

namespace PawnLabs.Dpay.Data
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

            return services;
        }
    }
}
