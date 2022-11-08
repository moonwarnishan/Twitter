namespace RegisterUser.InterFaces
{
    public interface ISearchServiceMongo 
    {
        

        //add new
        public Task createNewSearch(UserSearch userSearch);

    }
}
