using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationProvider.Interface
{
    public interface IConfigurationDatasource
    {
        event Action<Config> OnConfigurationChanged;
        Task<bool> UpsertAsync(Config config);
        Task<Config> GetAsync(string applicationName, string configurationName);
        Task<List<Config>> GetAllAsync();
        Task<bool> DeleteAsync(string id);
        Task Subscribe();
    }
}