namespace TweetRetweetCommentLike.Interfaces
{
    public interface IFollowBlockServices
    {
        Task Create(string folloeName, string followerName);
        Task Delete(string folloeName, string followerName);
        Task<List<string>> GetFollowedUsers(string folloeName);
        Task<bool> IsFollowed(string followerName, string folloeName);
        public Task CreateBlock(string blockUserName, string blockedUserName);
        public Task DeleteBlock(string blockUserName, string blockedUserName);
        public Task<List<string>> GetBlockedUsers(string blockUserName);
        public Task<bool> IsBlocked(string blockUserName, string blockedUserName);
        public Task<int> GetNumberOfFollowings(string folloeName);
    }
}
