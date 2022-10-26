namespace RegisterUser.InterFaces
{
    public interface ISearchServiceMongo
    {
        public Task createNewSearch(UserSearch userSearch);
        public Task delete(string key);
    }
}
