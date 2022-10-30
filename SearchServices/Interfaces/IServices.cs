namespace SearchServices.Interfaces
{
    public interface IServices
    {
        public Task<List<UserDto>> Users(string keyword);
        public Task<List<HashResponseDto>> Hashes(string keyword);
    }
}
