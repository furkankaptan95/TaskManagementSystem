using MongoDB.Driver;
using MongoDB.Bson;
using TaskAPI.Entities;

namespace TaskAPI.Services;
public class MongoDbService
{
    private readonly IMongoCollection<TaskEntity> _tasksCollection;
    private readonly IMongoCollection<UserEntity> _usersCollection;
    private readonly IMongoCollection<QuestionEntity> _questionsCollection;
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration config)
    {
        var client = new MongoClient(config.GetValue<string>("MongoDbSettings:ConnectionString"));
        _database = client.GetDatabase(config.GetValue<string>("MongoDbSettings:DatabaseName"));

        // Koleksiyonlar
        _tasksCollection = _database.GetCollection<TaskEntity>("tasks");
        _usersCollection = _database.GetCollection<UserEntity>("users");
        _questionsCollection = _database.GetCollection<QuestionEntity>("questions");

    }

    public IMongoCollection<TaskEntity> Tasks => _tasksCollection;
    public IMongoCollection<UserEntity> Users => _usersCollection;
    public IMongoCollection<QuestionEntity> Questions => _questionsCollection;
    public async Task CreateUserAsync(UserEntity user)
    {
        await _usersCollection.InsertOneAsync(user);
    }
    public async Task DeleteUserAsync(FilterDefinition<UserEntity> filter)
    {
        await _usersCollection.DeleteOneAsync(filter);
    }
    public async Task CreateTaskAsync(TaskEntity task)
    {
        await _tasksCollection.InsertOneAsync(task);
    }

    public async Task AddQuestionAsync(QuestionEntity question)
    {
        await _questionsCollection.InsertOneAsync(question);
    }

    public async Task<List<TaskEntity>> GetAllTasksAsync()
    {
        var tasks = await _tasksCollection.Find(task => true).ToListAsync();

        foreach (var task in tasks)
        {
            if(task.UserId is not null)
            {
                var userFilter = Builders<UserEntity>.Filter.Eq(user => user.Id , new ObjectId(task.UserId));
                var user = await GetUserAsync(userFilter);
                task.User = user;
            }
        }

        return tasks;
    }

    public async Task<UserEntity> GetUserAsync(FilterDefinition<UserEntity> filter)
    {
        return await _usersCollection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<QuestionEntity> GetQuestionAsync(FilterDefinition<QuestionEntity> filter)
    {
        return await _questionsCollection.Find(filter).FirstOrDefaultAsync();
    }
    public async Task<List<QuestionEntity>> GetTasksQuestionsAsync(FilterDefinition<QuestionEntity> filter)
    {
        return await _questionsCollection.Find(filter).ToListAsync();
    }
    public async Task<TaskEntity> GetTaskAsync(FilterDefinition<TaskEntity> filter)
    {
        // Görevi bul
        var task = await _tasksCollection.Find(filter).FirstOrDefaultAsync();

        if (task != null)
        {
            // Görev ile ilişkili kullanıcıyı bul
            if(task.UserId != null)
            {
                var user = await _usersCollection.Find(u => u.Id == new ObjectId(task.UserId)).FirstOrDefaultAsync();

                // Kullanıcı bilgilerini göreve ekle
                if (user != null)
                {
                    task.User = user;  // User bilgilerini ilişkilendir
                }
            }
        }

        return task;
    }

    public async Task UpdateTaskAsync(string id, TaskEntity task)  // ID'yi ObjectId olarak alıyoruz
    {
        var filter = Builders<TaskEntity>.Filter.Eq(t => t.Id, new ObjectId(id));
        await _tasksCollection.ReplaceOneAsync(filter, task);
    }

    public async Task UpdateQuestionAsync(string id, QuestionEntity question)  // ID'yi ObjectId olarak alıyoruz
    {
        var filter = Builders<QuestionEntity>.Filter.Eq(t => t.Id, new ObjectId(id));

        await _questionsCollection.ReplaceOneAsync(filter, question);
    }

    public async Task DeleteTaskAsync(string id)  // ID'yi ObjectId olarak alıyoruz
    {
        var filter = Builders<TaskEntity>.Filter.Eq(t => t.Id, new ObjectId(id));
        await _tasksCollection.DeleteOneAsync(filter);
    }
}
