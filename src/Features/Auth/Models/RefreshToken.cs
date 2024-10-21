using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HUBTSOCIAL.src.Features.Auth.Models
{
    public class RefreshToken
    {
        [BsonId] 
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId(); 
        public string UserId { get; set; } = String.Empty;
        public string AccessToken { get; set; } = String.Empty;
        public DateTime Expires { get; set; }
    }
}
