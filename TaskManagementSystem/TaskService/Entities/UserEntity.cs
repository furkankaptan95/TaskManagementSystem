using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskAPI.Entities;
public class UserEntity
{
    [BsonId]  // MongoDB'nin otomatik olarak ürettiği _id alanını işaret eder
    public ObjectId Id { get; set; }  // MongoDB'deki ObjectId türünde olacak

    public string Username { get; set; }
    public string Email { get; set; }

    // Kullanıcı ile ilişkili görevleri burada saklamıyoruz
    [BsonIgnore]  // Bu property MongoDB'ye kaydedilmez
    public  ICollection<TaskEntity> Tasks { get; set; }  // MongoDB koleksiyonunda yer almaz
}
