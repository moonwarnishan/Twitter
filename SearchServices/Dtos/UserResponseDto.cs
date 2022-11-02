using MongoDB.Bson.Serialization.Attributes;

namespace SearchServices.Dtos
{
    public class UserResponseDto
    {
        [BsonId]
        public string _id { get; set; }
        public string value { get; set; }
    }
}
