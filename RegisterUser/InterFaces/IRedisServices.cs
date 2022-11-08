namespace RegisterUser.InterFaces
{
    public interface IRedisServices
    {
        public Task SetCacheValueAsync(string Name);
    }
}
