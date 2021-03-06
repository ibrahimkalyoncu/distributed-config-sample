﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationProvider.Interface;
using ConfigurationProvider.Internal;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using StackExchange.Redis;

namespace ConfigurationProvider
{
    public class MongoConfigurationDatasource : IConfigurationDatasource
    {
        private bool _isSubscribing;
        private readonly ConfigurationProviderSettings _settings;
        private readonly ConfigurationMongoRepository _repository;

        private static ISubscriber _subscriber;
        private static ConnectionMultiplexer _connectionMultiplexer;
        private static readonly object LockObject = new object();

        public event Action<string> OnConfigurationChanged;

        public MongoConfigurationDatasource(IConfiguration configurationService)
        {
            _settings = configurationService.GetSection(Constants.AppConfigSection).Get<ConfigurationProviderSettings>();
            _repository = new ConfigurationMongoRepository(_settings.MongoConnectionString);
        }

        public async Task<List<Config>> GetAllAsync()
        {
            return (await _repository.GetAllAsync()).Select(Mapper.Map).ToList();
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var configToDelete = await _repository.GetAsync(id);

            if (configToDelete == null)
            {
                return false;
            }

            var isDeleted = await _repository.DeleteAsync(ObjectId.Parse(id));

            if (isDeleted)
            {
                //Always check subscriber if still alive
                if (_subscriber.Multiplexer.IsConnected == false)
                {
                    EnsureRedisConnection(_settings);
                }

                //Publish the change
                await _subscriber.PublishAsync(Constants.RedisPubSubChannel, configToDelete.Name);
            }

            return isDeleted;
        }

        //Enables PubSub via a redis server
        public async Task Subscribe()
        {
            if (_isSubscribing && _connectionMultiplexer.IsConnected)
                return;

            _isSubscribing = true;

            EnsureRedisConnection(_settings);
            await _subscriber.SubscribeAsync(Constants.RedisPubSubChannel, (channel, value) =>
            {
                if (value.HasValue)
                {
                    //Notify update via an event
                    OnConfigurationChanged?.Invoke(value.ToString());
                }
            });
        }

        public async Task<bool> UpsertAsync(Config configuration)
        {
            /*
             * Check if allready exist 
             */
            var config = string.IsNullOrEmpty(configuration.Id)
                ? await _repository.FindOneAsync(c => c.ApplicationName == configuration.ApplicationName && c.Name == configuration.Name)
                : await _repository.GetAsync(configuration.Id);

            /*
             * Map request to entity
             * A better solution is creating a mapper class 
             * because of limited time I choosed this way
             */
            var mappedConfig = new ConfigEntity
            {
                Name = configuration.Name,
                Value = configuration.Value,
                ApplicationName = configuration.ApplicationName,
                Type = configuration.Type,
                IsActive = configuration.IsActive
            };

            //Create new record
            if (config == null)
            {
                mappedConfig._id = ObjectId.GenerateNewId();
                return await _repository.InsertAsync(mappedConfig);
            }

            //Update existing record
            var isUpdated = await _repository.UpdateAsync(config._id.ToString(), mappedConfig);

            if (isUpdated && _isSubscribing)
            {
                //Always check subscriber if still alive
                if (_subscriber.Multiplexer.IsConnected == false)
                {
                    EnsureRedisConnection(_settings);
                }

                //Publish the change
                await _subscriber.PublishAsync(Constants.RedisPubSubChannel, mappedConfig.Name);
            }

            return isUpdated;

        }

        public async Task<Config> GetAsync(string applicationName, string configurationName)
        {
            return Mapper.Map(await _repository.FindOneAsync(c => c.IsActive && c.ApplicationName == applicationName && c.Name == configurationName));
        }

        private static void EnsureRedisConnection(ConfigurationProviderSettings settings)
        {
            if (_connectionMultiplexer == null || _connectionMultiplexer.IsConnected == false)
            {
                lock (LockObject)
                {
                    if (_connectionMultiplexer == null || _connectionMultiplexer.IsConnected == false)
                    {
                        _connectionMultiplexer = ConnectionMultiplexer.Connect(settings.RedisConnectionString);
                        _subscriber = _connectionMultiplexer.GetSubscriber();
                    }
                }
            }
        }
    }
}
