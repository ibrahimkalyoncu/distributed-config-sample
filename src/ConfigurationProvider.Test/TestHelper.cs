using System;
using ConfigurationProvider.Interface;
using Moq;

namespace ConfigurationProvider.Test
{
    public static class TestHelper
    {
        public static Config CreateConfiguration(string name, string value, string applicationName, bool isActive, string type)
        {
            return new Config
            {
                Name = name,
                Value = value,
                IsActive = isActive,
                ApplicationName = applicationName,
                Type = type,
                Id = Guid.NewGuid().ToString()
            };
        }
    }
}
