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

    public async Task<RefreshTokenEntity> GetRefreshTokenAsync(FilterDefinition<RefreshTokenEntity> filter)
    {
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

    public async Task UpdateManyRefreshTokensAsync(FilterDefinition<RefreshTokenEntity> filter, UpdateDefinition<RefreshTokenEntity> update)
    {
        await _refreshTokensCollection.UpdateManyAsync(filter, update);
    }

    public async Task UpdateSingleRefreshTokenAsync(FilterDefinition<RefreshTokenEntity> filter, UpdateDefinition<RefreshTokenEntity> update)
    {
       await _refreshTokensCollection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteUserVerificationAsync(FilterDefinition<UserVerificationEntity> filter)
    {
        await _userVerificationsCollection.DeleteOneAsync(filter);
    }
}

