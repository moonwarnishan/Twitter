namespace RegisterUser.InterFaces
{
    public interface IUserServices
    {
        public Task CreateAsync(UserInfo newUser);
        public Task<UserInfo?> GetUserByIdAsync(string id);
        public Task UpdateAsync(UserInfo updatedUser);
        public Task<List<UserInfo>> GetAllUsersAsync();
        public IMongoCollection<UserInfo> Users();
        public Task<UserInfo> FindByuserNameAsync(string userName);
        public UserInfo LoginValidation(LoginModel M);
        public UserInfo FindByuserName(string userName);
        public UserInfo FindByEmail(string email);
    }
}
