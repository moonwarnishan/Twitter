namespace TweetRetweetCommentLike.Interfaces
{
    public interface IFollowBlockIndividualServices
    {
        public  Task CreateFollowee(string userName, string followeeName);
        public Task DeleteFollowee(string userName, string followeeName);
        public Task<List<string>> GetFollowees(string userName);
        public Task<bool> IsFollowee(string userName, string followeeName);
        public  Task CreateBlockee(string userName, string blockeeName);
        public Task DeleteBlockee(string userName, string blockeeName);
        public Task<List<string>> GetBlockees(string userName);
        public  Task<bool> IsBlockee(string userName, string blockeeName);
        public Task<List<string>> GetAllFollowers(string userName);
        public Task<int> NumberOfFollowers(string userName);

    }
}
