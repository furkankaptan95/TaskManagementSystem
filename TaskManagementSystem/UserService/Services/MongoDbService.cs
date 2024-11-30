using MongoDB.Driver;
using UserAPI.Entities;

namespace UserAPI.Services;
public class MongoDbService
{
    private readonly IMongoCollection<UserEntity> _usersCollection;
    private readonly IMongoDatabase _database;
    public MongoDbService(IConfiguration config)
    {
        var client = new MongoClient(config.GetValue<string>("MongoDbSettings:ConnectionString"));
        _database = client.GetDatabase(config.GetValue<string>("MongoDbSettings:DatabaseName"));

        _usersCollection = _database.GetCollection<UserEntity>("users");
    }

    public IMongoCollection<UserEntity> Users => _usersCollection;

    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _usersCollection.Find(user => true).ToListAsync();
    }

    public async Task<UserEntity> GetUserAsync(FilterDefinition<UserEntity> filter)
    {
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateUserAsync(FilterDefinition<UserEntity> filter, UpdateDefinition<UserEntity> update)
    {
        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteUserAsync(FilterDefinition<UserEntity> filter)
    {
        await _usersCollection.DeleteOneAsync(filter);
    }
}
