namespace RegisterUser.Validators
{
    public class UserNameUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(
            object? value, ValidationContext validationContext)
        {
            UserServices? _context = validationContext.GetService(serviceType: typeof(UserServices)) as UserServices;
            //var entity = _context?.Users().Find(x => x.userName.ToUpper() == value.ToString().ToUpper()).FirstOrDefault();
            var entity = _context.FindByuserName(value.ToString().ToLower());
            if (entity == null)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(GetErrorMessage(UserName: value.ToString()));
        }

        public string GetErrorMessage(string UserName)
        {
            return $"User Name {UserName} is already in use.";
        }
    }
}
