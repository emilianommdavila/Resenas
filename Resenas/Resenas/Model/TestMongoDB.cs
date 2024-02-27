using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Resenas.Model
{
    public class TestMongoDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int Id { get; set; }
        [BsonElement("userID")]

        public int userID { get; set; }
        [BsonElement("content")]

        public string content { get; set; }
    }
}
