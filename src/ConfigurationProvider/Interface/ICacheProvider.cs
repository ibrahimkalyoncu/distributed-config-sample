namespace ConfigurationProvider.Interface
{
    public interface ICacheProvider
    {
        Config Get(string key);
        void Set(string key, Config value);
        void Invalidate(string key);
    }
}