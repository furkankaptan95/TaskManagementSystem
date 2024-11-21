using MongoDB.Bson;
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

        // Koleksiyon
        _usersCollection = _database.GetCollection<UserEntity>("users");

    }

    public IMongoCollection<UserEntity> Users => _usersCollection;

    // Read - Tüm kullanıcıları getirir
    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _usersCollection.Find(user => true).ToListAsync();
    }

    // Read - ID'ye göre bir kullanıcı getirir
    public async Task<UserEntity> GetUserByIdAsync(string id)  // ObjectId olarak alınacak
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, new ObjectId(id));
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }

    // Update - Kullanıcı bilgilerini günceller
    public async Task UpdateUserAsync(string id, UserEntity user)
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, new ObjectId(id));
        await _usersCollection.ReplaceOneAsync(filter, user);
    }

    // Delete - Kullanıcıyı siler
    public async Task DeleteUserAsync(string id)
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, new ObjectId(id));
        await _usersCollection.DeleteOneAsync(filter);
    }
}
