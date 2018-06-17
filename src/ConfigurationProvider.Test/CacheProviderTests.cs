using ConfigurationProvider.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace ConfigurationProvider.Test
{
    [TestClass]
    public class CacheProviderTests
    {
        private const string ApplicationName = "Test";
        private readonly ICacheProvider _cacheProvider;

        public CacheProviderTests()
        {
            _cacheProvider = new CacheProvider();
        }

        [TestMethod]
        public void Should_Set_And_Get_Value()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "TestValue", ApplicationName, true, typeof(string).Name);

            //Act
            _cacheProvider.Set(configuration.Name, configuration);
            var cachedConfiguration = _cacheProvider.Get(configuration.Name);

            //Assert
            cachedConfiguration.ShouldNotBeNull();
            cachedConfiguration.Name.ShouldBe(configuration.Name);
            cachedConfiguration.Value.ShouldBe(configuration.Value);
        }

        [TestMethod]
        public void Should_Invalidate_Cache()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "TestValue", ApplicationName, true, typeof(string).Name);

            //Act
            _cacheProvider.Set(configuration.Name, configuration);
            _cacheProvider.Invalidate(configuration.Name);
            var cachedConfiguration = _cacheProvider.Get(configuration.Name);

            //Assert
            cachedConfiguration.ShouldBeNull();
        }
    }
}