using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConfigurationProvider.Internal
{
    internal class ConfigurationMongoRepository
    {
        private readonly IMongoCollection<ConfigEntity> _collection;

        public ConfigurationMongoRepository(string connectionString)
        {
            IMongoClient client = new MongoClient(connectionString);
            var database = client.GetDatabase(Constants.MongoDatabase);
            _collection = database.GetCollection<ConfigEntity>(Constants.MongoCollection);
        }

        public async Task<bool> InsertAsync(ConfigEntity configEntity)
        {
            await _collection.InsertOneAsync(configEntity);
            return await _collection.CountAsync(c => c._id.Equals(configEntity._id)) > 0;
        }

        public async Task<ConfigEntity> FindOneAsync(Expression<Func<ConfigEntity, bool>> predicate)
        {
            return await (await _collection.FindAsync<ConfigEntity>(predicate)).SingleOrDefaultAsync();
        }

        public async Task<ConfigEntity> GetAsync(string id)
        {
            return await _collection.Find(new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync();
        }

        public async Task<bool> UpdateAsync(string id, ConfigEntity postmodel)
        {
            postmodel._id = ObjectId.Parse(id);

            var filter = Builders<ConfigEntity>.Filter.Eq(s => s._id, postmodel._id);
            return (await _collection.ReplaceOneAsync(filter, postmodel)).ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            return (await _collection.DeleteOneAsync(c => c._id.Equals(id))).DeletedCount > 0;
        }

        public async Task<List<ConfigEntity>> GetAllAsync()
        {
            return (await _collection.FindAsync(FilterDefinition<ConfigEntity>.Empty)).ToList();
        }
    }
}