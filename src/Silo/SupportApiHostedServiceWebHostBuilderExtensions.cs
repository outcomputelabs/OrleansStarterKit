using Microsoft.Extensions.DependencyInjection;
using Silo.Options;
using System;

namespace Silo
{
    public static class SupportApiHostedServiceWebHostBuilderExtensions
    {
        public static IServiceCollection AddSupportApiInfo(this IServiceCollection services, string title)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Configure<SupportApiOptions>(options =>
            {
                options.Title = title;
            });

            return services;
        }
    }
}
