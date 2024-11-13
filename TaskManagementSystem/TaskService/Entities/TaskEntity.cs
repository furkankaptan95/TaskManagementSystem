using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TaskAPI.Entities
{
    public class TaskEntity
    {
        [BsonId]  // MongoDB'nin otomatik oluşturduğu _id alanını işaret ederiz
        public ObjectId Id { get; set; }  // MongoDB'deki ObjectId türünde olacak

        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }

        // MongoDB'deki UserId ObjectId türünde olacak
        [BsonRepresentation(BsonType.ObjectId)]  // ObjectId türüne dönüştürme
        public string? UserId { get; set; }  // UserId'yi ObjectId türünde tutuyoruz

        // İlişkili kullanıcıyı belirtmek, ancak bu veritabanına kaydedilmeyecek
        [BsonIgnore]  // MongoDB veritabanına kaydedilmez
        public UserEntity? User { get; set; }  // Kullanıcı nesnesi burada yer almaz

    }
}
