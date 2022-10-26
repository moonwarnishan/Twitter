
namespace RegisterUser.Helpers
{
    public class PasswordHash
    {
        public static string HashPassword(string password)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                password = String.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(password))
                    .Select(item => item.ToString("x2")));
            }
            return password;
        }
    }
}
