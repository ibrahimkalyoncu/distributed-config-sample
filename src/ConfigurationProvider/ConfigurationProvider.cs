using System;
using System.Threading.Tasks;
using ConfigurationProvider.Interface;
using ConfigurationProvider.Internal;
using Microsoft.Extensions.Configuration;

namespace ConfigurationProvider
{
    public class ConfigurationProvider : Interface.IConfigurationProvider
    {
        private readonly string _applicationName;
        private readonly IConfigurationDatasource _dataSource;
        private readonly ICacheProvider _cacheProvider;

        public ConfigurationProvider(IConfiguration options, IConfigurationDatasource dataSource, ICacheProvider cacheProvider)
        {
            _applicationName = options.GetSection(Constants.AppConfigSection).Get<ConfigurationProviderSettings>().ApplicationName;
            _dataSource = dataSource;
            _cacheProvider = cacheProvider;

            _dataSource.OnConfigurationChanged += configuration =>
            {
                _cacheProvider.Invalidate(configuration.Name);
            };
        }

        public async Task<T> GetAsync<T>(string key) where T : IConvertible
        {
            Config configuration = _cacheProvider.Get(key) ?? await _dataSource.GetAsync(_applicationName, key);

            if (configuration == null)
            {
                return default(T);
            }

            var conversionType = typeof(T);

            if (configuration.Type != conversionType.Name)
            {
                throw new InvalidCastException($"Configuration type {configuration.Type} is different than expected type {conversionType.Name}");
            }

            var convertedValue = (T)Convert.ChangeType(configuration.Value, conversionType);
            _cacheProvider.Set(key, configuration);
            return convertedValue;
        }
    }
}
