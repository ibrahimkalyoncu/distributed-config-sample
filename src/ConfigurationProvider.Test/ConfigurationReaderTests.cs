using System;
using System.Threading.Tasks;
using ConfigurationProvider.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using IConfigurationProvider = ConfigurationProvider.Interface.IConfigurationProvider;

namespace ConfigurationProvider.Test
{
    [TestClass]
    public class ConfigurationReaderTests
    {
        private readonly IConfigurationProvider _provider;
        private readonly Mock<ICacheProvider> _cacheProvider;
        private readonly ConfigurationProviderSettings _settings;
        private readonly Mock<IConfigurationDatasource> _dataSourceMock;

        public ConfigurationReaderTests()
        {
            _cacheProvider = new Mock<ICacheProvider>();
            _dataSourceMock = new Mock<IConfigurationDatasource>();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _settings = configuration.Get<ConfigurationProviderSettings>();

            _provider = new ConfigurationProvider(configuration, _dataSourceMock.Object, _cacheProvider.Object);
        }

        [TestMethod]
        public async Task Should_Get_String_Value()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "TestValue", _settings.ApplicationName, true, typeof(string).Name);

            _dataSourceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(configuration);

            //Act
            string value = await _provider.GetAsync<string>(configuration.Name);

            //Assert
            value.ShouldBe(configuration.Value);
        }

        [TestMethod]
        public async Task Should_Get_Integer_Value()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "5", _settings.ApplicationName, true, typeof(int).Name);
            _dataSourceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(configuration);

            //Act
            int value = await _provider.GetAsync<int>(configuration.Name);

            //Assert
            value.ShouldBe(5);
        }

        [TestMethod]
        public async Task Should_Throw_Format_Exception_When_Casting_Invalid_Input()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "Five", _settings.ApplicationName, true, typeof(int).Name);
            _dataSourceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(configuration);

            //Act
            //Assert
            await Assert.ThrowsExceptionAsync<FormatException>(async () => await _provider.GetAsync<int>(configuration.Name));
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_Type_Mismatch()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "5", _settings.ApplicationName, true, typeof(int).Name);
            _dataSourceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(configuration);

            //Act
            //Assert
            await Assert.ThrowsExceptionAsync<InvalidCastException>(async () => await _provider.GetAsync<string>(configuration.Name));
        }

        [TestMethod]
        public async Task Should_Get_Value_From_Cache_When_Datasource_Is_Not_Available()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "5", _settings.ApplicationName, true, typeof(int).Name);

            _cacheProvider.Setup(x => x.Get(configuration.Name)).Returns(configuration);
            _dataSourceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            //Act
            int value = await _provider.GetAsync<int>(configuration.Name);

            //Assert
            value.ToString().ShouldBe(configuration.Value);
        }

        [TestMethod]
        public async Task Should_Throw_Exception_When_Cache_Is_Empty_And_Datasource_Is_Not_Available()
        {
            //Arrange
            var configuration = TestHelper.CreateConfiguration("TestName", "5", _settings.ApplicationName, true, typeof(int).Name);

            _dataSourceMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            //Act
            //Assert
            await Assert.ThrowsExceptionAsync<Exception>(async () => await _provider.GetAsync<int>(configuration.Name));
        }
    }
}
