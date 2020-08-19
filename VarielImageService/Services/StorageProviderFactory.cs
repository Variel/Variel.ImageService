using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Variel.ImageService.Services.StorageProviders;

namespace Variel.ImageService.Services
{
    public class StorageProviderFactory
    {
        private static readonly Dictionary<string, Type> _providerDictionary;

        static StorageProviderFactory()
        {
            _providerDictionary = (from t in Assembly.GetExecutingAssembly().GetTypes()
                    let name = t.GetCustomAttribute<StorageProviderAttribute>()?.Name
                    where typeof(IStorageProvider).IsAssignableFrom(t) && !String.IsNullOrWhiteSpace(name)
                    select new {t, name})
                .ToDictionary(v => v.name, v => v.t);

        }

        private readonly IServiceProvider _serviceProvider;

        public StorageProviderFactory(IServiceProvider provider)
        {
            _serviceProvider = provider;
        }

        public IStorageProvider GetProviderAsync(string providerName, string settingsJson)
        {
            if (!_providerDictionary.TryGetValue(providerName, out var providerType))
            {
                throw new KeyNotFoundException("Not existing storage provider name");
            }

            var provider = (IStorageProvider) _serviceProvider.GetService(providerType);
            provider.Initialize(settingsJson);

            return provider;
        }
    }
}
