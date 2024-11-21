using AuthAPI.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthAPI.Services;
public class MongoDbService
{
    private readonly IMongoCollection<UserEntity> _usersCollection;
    private readonly IMongoCollection<RefreshTokenEntity> _refreshTokensCollection;
    private readonly IMongoDatabase _database;
    public MongoDbService(IConfiguration config)
    {
        var client = new MongoClient(config.GetValue<string>("MongoDbSettings:ConnectionString"));
        _database = client.GetDatabase(config.GetValue<string>("MongoDbSettings:DatabaseName"));

        // Koleksiyon
        _usersCollection = _database.GetCollection<UserEntity>("users");
        _refreshTokensCollection = _database.GetCollection<RefreshTokenEntity>("refreshTokens");

    }

    public IMongoCollection<UserEntity> Users => _usersCollection;
    public IMongoCollection<RefreshTokenEntity> RefreshTokens => _refreshTokensCollection;
    // Create - Yeni bir kullanıcı ekler
    public async Task CreateRefreshTokenAsync(RefreshTokenEntity token)
    {
        await _refreshTokensCollection.InsertOneAsync(token);
    }
    public async Task<RefreshTokenEntity> GetRefreshTokenAsync(string token)
    {
        var filter = Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.Token, token) &
              Builders<RefreshTokenEntity>.Filter.Gt(rt => rt.ExpireDate, DateTime.UtcNow) &
              Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.IsRevoked, false) &
              Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.IsUsed, false);

        var refreshTokenEntity = await _refreshTokensCollection
            .Find(filter)
            .FirstOrDefaultAsync();

        if(refreshTokenEntity is null)
        {
            return null;
        }

        var user = await GetUserByIdAsync(refreshTokenEntity.UserId);
        refreshTokenEntity.User = user;

        var updateFilter = Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.Token, token);
        var update = Builders<RefreshTokenEntity>.Update.Set(rt => rt.IsUsed, true);

        await _refreshTokensCollection.UpdateOneAsync(updateFilter, update);

        return refreshTokenEntity;
    }

    public async Task<UserEntity> GetUserByIdAsync(string id)
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, new ObjectId(id));
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }
    // Read - Tüm kullanıcıları getirir
    public async Task<List<UserEntity>> GetAllUsersAsync()
    {
        return await _usersCollection.Find(user => true).ToListAsync();
    }

    public async Task<List<RefreshTokenEntity>> GetAllUserTokensAsync(string userId)
    {
        return await _refreshTokensCollection.Find(token => token.UserId == userId).ToListAsync();
    }

    public async Task<UserEntity> GetUserWithTokensAsync(string email)
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Email, email);
        var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();

        if (user is not null)
        {

            user.RefreshTokens = await GetAllUserTokensAsync(user.Id.ToString());

        }

        return user;
    }

    public async Task UpdateAllTokensToRevokedAsync(string userId)
    {
        // Güncelleme filtresi: userId eşleşenler
        var filter = Builders<RefreshTokenEntity>.Filter.Eq(token => token.UserId, userId);

        // Güncelleme işlemi: IsRevoked alanını false yap
        var update = Builders<RefreshTokenEntity>.Update.Set(token => token.IsRevoked, true);

        // Güncelleme işlemini uygula
        var result = await _refreshTokensCollection.UpdateManyAsync(filter, update);

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

