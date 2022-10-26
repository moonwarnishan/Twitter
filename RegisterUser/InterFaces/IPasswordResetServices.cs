namespace RegisterUser.InterFaces
{
    public interface IPasswordResetServices
    {
        public string RequestPasswordReset(string userEmail);
        public Task ResetPassword(ResetPasswordDto resetPasswordDto);

    }
}
