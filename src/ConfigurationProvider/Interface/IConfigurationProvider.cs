using System;
using System.Threading.Tasks;

namespace ConfigurationProvider.Interface
{
    public interface IConfigurationProvider
    {
        Task<T> GetAsync<T>(string key) where T : IConvertible;
    }
}
