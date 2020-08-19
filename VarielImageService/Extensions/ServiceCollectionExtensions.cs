using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Variel.ImageService.Services;
using Variel.ImageService.Services.StorageProviders;

namespace Variel.ImageService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddStorageProviders(this IServiceCollection self)
        {
            var providerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IStorageProvider).IsAssignableFrom(t) && t != typeof(IStorageProvider));

            foreach (var t in providerTypes)
            {
                self.AddScoped(t);
            }

            self.AddScoped<StorageProviderFactory>();
        }
    }
}
