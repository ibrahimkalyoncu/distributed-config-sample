using ConfigurationProvider.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationProvider.Extensions
{
    public static class Extensions
    {
        public static void AddConfiguration(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<Interface.IConfigurationDatasource, MongoConfigurationDatasource>();
            serviceCollection.AddSingleton<Interface.IConfigurationProvider, ConfigurationProvider>();
            serviceCollection.AddSingleton<Interface.ICacheProvider, CacheProvider>();
        }

        public static void UseConfiguration(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.ApplicationServices.GetService<IConfigurationDatasource>().Subscribe();
        }
    }
}
