namespace RegisterUser.InterFaces
{
    public interface ISearchServiceRedis
    {
        public Task SetUserSearchValue(string userName, string name, string userId);
        public Task DeleteKey(string userName, string name);
    }
}
