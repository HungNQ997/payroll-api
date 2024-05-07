using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Payroll.Data.Dtos.Users
{
    public class UserInfoDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("userName")] // Map to MongoDB field "userName"
        public string UserName { get; set; }

        [BsonElement("password")] // Map to MongoDB field "password"
        public string Password { get; set; }

        [BsonElement("fullName")] // Map to MongoDB field "fullName"
        public string FullName { get; set; }

        [BsonElement("description")] // Map to MongoDB field "description"
        public string Description { get; set; }

        [BsonElement("tokenLifeTimeMinutes")] // Map to MongoDB field "tokenLifeTimeMinutes"
        public long? TokenLifeTimeMinutes { get; set; }

        [BsonElement("isActive")] // Map to MongoDB field "isActive"
        public int IsActive { get; set; }
        [BsonElement("isDeleted")] // Map to MongoDB field "isActive"
        public int IsDeleted { get; set; }

        [BsonElement("role")] // Map to MongoDB field "role"
        public string[] RoleNames { get; set; }
    }
}
