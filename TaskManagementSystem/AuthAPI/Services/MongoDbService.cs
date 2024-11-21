using AuthAPI.DTOs;
using AuthAPI.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthAPI.Services;
public class MongoDbService
{
    private readonly IMongoCollection<UserEntity> _usersCollection;
    private readonly IMongoCollection<RefreshTokenEntity> _refreshTokensCollection;
    private readonly IMongoCollection<UserVerificationEntity> _userVerificationsCollection;
    private readonly IMongoDatabase _database;
    public MongoDbService(IConfiguration config)
    {
        var client = new MongoClient(config.GetValue<string>("MongoDbSettings:ConnectionString"));
        _database = client.GetDatabase(config.GetValue<string>("MongoDbSettings:DatabaseName"));

        // Koleksiyon
        _usersCollection = _database.GetCollection<UserEntity>("users");
        _refreshTokensCollection = _database.GetCollection<RefreshTokenEntity>("refreshTokens");
        _userVerificationsCollection = _database.GetCollection<UserVerificationEntity>("userVerifications");

    }

    public IMongoCollection<UserEntity> Users => _usersCollection;
    public IMongoCollection<RefreshTokenEntity> RefreshTokens => _refreshTokensCollection;
    public IMongoCollection<UserVerificationEntity> UserVerficications => _userVerificationsCollection;
    public async Task CreateUserAsync(UserEntity user)
    {
        await _usersCollection.InsertOneAsync(user);
    }
    public async Task CreateRefreshTokenAsync(RefreshTokenEntity token)
    {
        await _refreshTokensCollection.InsertOneAsync(token);
    }

    public async Task CreateUserVerificationAsync(UserVerificationEntity entity)
    {
        await _userVerificationsCollection.InsertOneAsync(entity);
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

        var userFilter = Builders<UserEntity>.Filter.Eq(u => u.Id , new ObjectId(refreshTokenEntity.UserId));

        var user = await GetUserAsync(userFilter);
        refreshTokenEntity.User = user;

        var updateFilter = Builders<RefreshTokenEntity>.Filter.Eq(rt => rt.Token, token);
        var update = Builders<RefreshTokenEntity>.Update.Set(rt => rt.IsUsed, true);

        await _refreshTokensCollection.UpdateOneAsync(updateFilter, update);

        return refreshTokenEntity;
    }

    public async Task<UserVerificationEntity> GetUserVerificationAsync(FilterDefinition<UserVerificationEntity> filter)
    {
        var userVerification = await _userVerificationsCollection.Find(filter).FirstOrDefaultAsync();

        if(userVerification is null)
        {
            return userVerification;
        }

        var userFilter = Builders<UserEntity>.Filter.Eq(u => u.Id, new ObjectId(userVerification.UserId));
        var user = await GetUserAsync(userFilter);

        userVerification.User = user;

        return userVerification;
    }

    public async Task UpdateUserAsync(FilterDefinition<UserEntity> filter, UpdateDefinition<UserEntity> update)
    {
        await _usersCollection.UpdateOneAsync(filter, update);
    }

    public async Task<UserEntity> GetUserAsync(FilterDefinition<UserEntity> filter)
    {
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

    // Delete - Kullanıcıyı siler
    public async Task DeleteUserAsync(string id)
    {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, new ObjectId(id));
        await _usersCollection.DeleteOneAsync(filter);
    }

    public async Task DeleteUserVerificationAsync(FilterDefinition<UserVerificationEntity> filter)
    {
        await _userVerificationsCollection.DeleteOneAsync(filter);
    }
}

