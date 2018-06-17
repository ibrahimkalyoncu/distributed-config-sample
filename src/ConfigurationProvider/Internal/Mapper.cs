namespace ConfigurationProvider.Internal
{
    internal static class Mapper
    {
        public static Config Map(ConfigEntity configEntity)
        {
            return configEntity == null
                ? null
                : new Config
                {
                    Id = configEntity._id.ToString(),
                    Name = configEntity.Name,
                    Value = configEntity.Value,
                    IsActive = configEntity.IsActive,
                    ApplicationName = configEntity.ApplicationName,
                    Type = configEntity?.Type
                };
        }
    }
}
