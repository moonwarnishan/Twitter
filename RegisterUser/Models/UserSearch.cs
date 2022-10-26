namespace RegisterUser.Services
{
    public class UserSearch
    {
        [BsonId]
        public string key { get; set; }
        public string value { get; set; }
    }
}