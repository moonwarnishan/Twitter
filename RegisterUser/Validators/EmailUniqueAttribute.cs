namespace RegisterUser.Validators
{
    public class EmailUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            UserServices? context = validationContext.GetService(typeof(UserServices)) as UserServices;
            var entity = context?.Users().Find(x => x.email.ToLower() == value.ToString().ToLower()).FirstOrDefault();

            if (entity != null)
            {
                return new ValidationResult(GetErrorMessage(value.ToString()));
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage(string email)
        {
            return $"Email {email} is already in use.";
        }
    }
}
