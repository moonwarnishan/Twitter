namespace RegisterUser.InterFaces
{
    public interface IUserServices
    {
        public Task CreateAsync(User newUser);
        public Task<User?> GetUserByIdAsync(string id);
        public Task UpdateAsync(User updatedUser);
        public Task<List<User>> GetAllUsersAsync();
        public IMongoCollection<User> Users();
        public Task<User> FindByuserNameAsync(string userName);
        public User LoginValidation(LoginModel M);
        public User FindByuserName(string userName);
        public User FindByEmail(string email);
    }
}
