using MongoDB.Driver;
using MongoDB.Bson;
using TaskAPI.Entities;

namespace TaskAPI.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<TaskEntity> _tasksCollection;
        private readonly IMongoCollection<UserEntity> _usersCollection;
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration config)
        {
            var client = new MongoClient(config.GetValue<string>("MongoDbSettings:ConnectionString"));
            _database = client.GetDatabase(config.GetValue<string>("MongoDbSettings:DatabaseName"));

            // Koleksiyonlar
            _tasksCollection = _database.GetCollection<TaskEntity>("tasks");
            _usersCollection = _database.GetCollection<UserEntity>("users");

            // Veritabanı ve Koleksiyonların varlığını kontrol et
            CreateDatabaseIfNotExists();
        }

        public IMongoCollection<TaskEntity> Tasks => _tasksCollection;
        public IMongoCollection<UserEntity> Users => _usersCollection;

        private void CreateDatabaseIfNotExists()
        {
            // Veritabanının var olup olmadığını kontrol et (MongoDB otomatik olarak oluşturur)
            var databaseNames = _database.Client.ListDatabaseNames().ToList();
            if (!databaseNames.Contains(_database.DatabaseNamespace.DatabaseName))
            {
                _database.Client.GetDatabase(_database.DatabaseNamespace.DatabaseName);
            }

            // Koleksiyonlar otomatik olarak oluşturulacaktır, ancak burada herhangi bir yapılandırma gerekiyorsa,
            // koleksiyonlar üzerinde bazı işlemler yapabiliriz.
            // Örneğin, 'tasks' ve 'users' koleksiyonlarının oluşturulması
            var taskCollectionExists = _database.ListCollectionNames().ToList().Contains("tasks");
            if (!taskCollectionExists)
            {
                // Burada opsiyonel olarak koleksiyon yapılandırmaları yapabilirsiniz.
                _database.CreateCollection("tasks");
            }

            var userCollectionExists = _database.ListCollectionNames().ToList().Contains("users");
            if (!userCollectionExists)
            {
                _database.CreateCollection("users");
            }
        }

        // Create - Yeni bir görev ekler
        public async Task CreateTaskAsync(TaskEntity task)
        {
            await _tasksCollection.InsertOneAsync(task);
        }

        // Read - Tüm görevleri getirir
        public async Task<List<TaskEntity>> GetAllTasksAsync()
        {
            return await _tasksCollection.Find(task => true).ToListAsync();
        }

        // Read - ID'ye göre bir görev getirir ve ilişkili kullanıcıyı da getirir
        public async Task<TaskEntity> GetTaskByIdAsync(string id)  // ID olarak string (ObjectId) alıyoruz
        {
            // Görevi bul
            var task = await _tasksCollection.Find(task => task.Id == new ObjectId(id)).FirstOrDefaultAsync();

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

        // Update - Görev bilgisini günceller
        public async Task UpdateTaskAsync(string id, TaskEntity task)  // ID'yi ObjectId olarak alıyoruz
        {
            var filter = Builders<TaskEntity>.Filter.Eq(t => t.Id, new ObjectId(id));
            await _tasksCollection.ReplaceOneAsync(filter, task);
        }

        // Delete - Görevi siler
        public async Task DeleteTaskAsync(string id)  // ID'yi ObjectId olarak alıyoruz
        {
            var filter = Builders<TaskEntity>.Filter.Eq(t => t.Id, new ObjectId(id));
            await _tasksCollection.DeleteOneAsync(filter);
        }
    }
}
