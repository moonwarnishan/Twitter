namespace RegisterUser.Validators
{
    public class UserNameExistanceCheck : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var nameProperty = validationContext.ObjectType.GetProperty("userName");
            var usernameValue = nameProperty.GetValue(validationContext.ObjectInstance);


            UserServices? _context = validationContext.GetService(typeof(UserServices)) as UserServices;
            //var entity = _context?.Users().Find(x => x.Email == value.ToString()).FirstOrDefault();
            var entity = _context?.Users().Find(x => x.email == value.ToString() && x.userName != usernameValue).FirstOrDefault();

            if (entity != null)
            {
                return new ValidationResult(GetErrorMessage(value.ToString()));
            }

            return ValidationResult.Success;
        }
        public string GetErrorMessage(string username)
        {
            return $"username {username} is already in use.";
        }
    }
}

