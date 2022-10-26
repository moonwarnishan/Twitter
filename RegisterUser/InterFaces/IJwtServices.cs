namespace RegisterUser.InterFaces
{
    public interface IJwtServices
    {
        public Task<ReturnToken> Authenticate(LoginModel M);
        public ReturnToken TokenRefresh(ReturnToken token);
        
    }
}
